namespace Rentar.Modules.Fleet.Infrastructure.Storage;

public class ImageStorageOptions
{
    public const string SectionName = "Paths";

    /// <summary>Uploads folder, relative to the content root unless absolute.</summary>
    public string DefaultCarImagePath { get; set; } = "wwwroot/uploads";
}
