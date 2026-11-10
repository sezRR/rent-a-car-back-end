using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rentar.Modules.Fleet.Application.CarImages;
using System;
using System.IO;
using System.Transactions;

namespace Rentar.Modules.Fleet.Infrastructure.Storage;

/// <summary>
/// Keeps uploaded images in the configured uploads folder on local disk.
/// </summary>
public class LocalImageStorage(IOptions<ImageStorageOptions> options, IHostEnvironment environment) : IImageStorage
{
    public string UploadsPath { get; } = Path.GetFullPath(options.Value.DefaultCarImagePath, environment.ContentRootPath);

    public string Save(FileUpload file)
    {
        Directory.CreateDirectory(UploadsPath);

        var fileName = NewFileName(file);
        var fullPath = Path.Combine(UploadsPath, fileName);

        using (var target = new FileStream(fullPath, FileMode.CreateNew))
        using (var source = file.OpenReadStream())
        {
            source.CopyTo(target);
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

    public string Replace(FileUpload file, string oldFileName)
    {
        var fileName = Save(file);

        Delete(oldFileName);

        return fileName;
    }

    public void Delete(string fileName)
    {
        // The default image is shared by every car without an upload, so it is never removed.
        if (string.IsNullOrEmpty(fileName) || fileName == IImageStorage.DefaultImageName)
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

    private static string NewFileName(FileUpload file)
    {
        return Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
    }
}
