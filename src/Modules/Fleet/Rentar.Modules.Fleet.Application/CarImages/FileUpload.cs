using System;
using System.IO;

namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>
/// An uploaded file as the application sees it, so this layer does not depend on ASP.NET's <c>IFormFile</c>.
/// </summary>
public sealed class FileUpload(string fileName, long length, Func<Stream> openReadStream)
{
    private readonly Func<Stream> _openReadStream = openReadStream;

    public string FileName { get; } = fileName;
    public long Length { get; } = length;

    /// <summary>Opens a fresh stream over the content; the caller disposes it.</summary>
    public Stream OpenReadStream()
    {
        return _openReadStream();
    }
}
