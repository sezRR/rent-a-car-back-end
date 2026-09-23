using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Transactions;

namespace Core.Utilities.FileHelper;

/// <summary>
/// Stores uploaded images in the configured uploads folder. Inside an ambient transaction the disk follows
/// the database: deleted files are only removed once the transaction commits, and added files are removed
/// again if it rolls back.
/// </summary>
public class FileHelper
{
    public const string DefaultImageName = "default.png";

    public static IConfiguration Configuration { get; set; }
    public static string BasePath { get; set; } = AppContext.BaseDirectory;

    public static string UploadsPath => Path.GetFullPath(Configuration["Paths:DefaultCarImagePath"], BasePath);

    public static string Add(IFormFile file)
    {
        Directory.CreateDirectory(UploadsPath);

        var fileName = NewFileName(file);
        var fullPath = Path.Combine(UploadsPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.CreateNew))
        {
            file.CopyTo(stream);
        }

        OnTransactionEnd(committed =>
        {
            if (!committed)
            {
                File.Delete(fullPath);
            }
        });

        return fileName;
    }

    public static string Update(IFormFile file, string oldFileName)
    {
        var fileName = Add(file);

        Delete(oldFileName);

        return fileName;
    }

    public static void Delete(string fileName)
    {
        // The default image is shared by every car without an upload, so it is never removed.
        if (string.IsNullOrEmpty(fileName) || fileName == DefaultImageName)
        {
            return;
        }

        // Only the file name is kept so a stored value can not point outside the uploads folder.
        var fullPath = Path.Combine(UploadsPath, Path.GetFileName(fileName));

        if (Transaction.Current == null)
        {
            File.Delete(fullPath);
            return;
        }

        OnTransactionEnd(committed =>
        {
            if (committed)
            {
                File.Delete(fullPath);
            }
        });
    }

    private static void OnTransactionEnd(Action<bool> action)
    {
        var transaction = Transaction.Current;
        if (transaction == null)
        {
            return;
        }

        transaction.TransactionCompleted += (_, e) =>
            action(e.Transaction.TransactionInformation.Status == TransactionStatus.Committed);
    }

    private static string NewFileName(IFormFile file)
    {
        return Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
    }
}
