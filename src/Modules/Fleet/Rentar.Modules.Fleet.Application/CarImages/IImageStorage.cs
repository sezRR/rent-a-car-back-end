namespace Rentar.Modules.Fleet.Application.CarImages;

/// <summary>
/// Where car image files live. Inside an ambient transaction the files follow the database: a deleted file
/// is only removed once the transaction commits, and a saved file is removed again if it rolls back.
/// </summary>
public interface IImageStorage
{
    /// <summary>Name of the shared placeholder image used by cars without an upload. It is never deleted.</summary>
    const string DefaultImageName = "default.png";

    /// <summary>Stores the file under a new unique name and returns that name.</summary>
    string Save(FileUpload file);

    /// <summary>Stores the file and removes the one it replaces; returns the new name.</summary>
    string Replace(FileUpload file, string oldFileName);

    void Delete(string fileName);
}
