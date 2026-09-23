using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Utilities.FileHelper;

/// <summary>
/// Accepts only PNG and JPEG uploads. The extension alone can be faked, so the file's leading bytes
/// must also carry the signature of the format the extension claims.
/// </summary>
public static class ImageFileValidator
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];

    private static readonly Dictionary<string, byte[]> SignaturesByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        [".png"] = PngSignature,
        [".jpg"] = JpegSignature,
        [".jpeg"] = JpegSignature
    };

    public static bool IsSupportedImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        if (!SignaturesByExtension.TryGetValue(extension, out var signature))
        {
            return false;
        }

        var header = new byte[signature.Length];
        using var stream = file.OpenReadStream();
        var read = stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false);

        return read == header.Length && header.SequenceEqual(signature);
    }
}
