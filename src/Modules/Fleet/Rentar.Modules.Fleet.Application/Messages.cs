namespace Rentar.Modules.Fleet.Application;

public static class Messages
{
    public const string BrandsListed = "Brands Listed";
    public const string BrandAdded = "Brand Added";
    public const string BrandUpdated = "Brand Updated";
    public const string BrandDeleted = "Brand Deleted";

    public const string CarsListed = "Cars Listed";
    public const string CarAdded = "Car Added";
    public const string CarUpdated = "Car Updated";
    public const string CarDeleted = "Car Deleted";
    public const string TheCarNotFound = "Desired car is not on the database";

    public const string ColorsListed = "Colors Listed";
    public const string ColorAdded = "Color Added";
    public const string ColorUpdated = "Color Updated";
    public const string ColorDeleted = "Color Deleted";
    public const string ColorNameInvalid = "Color Name is Invalid";

    public const string CarImageLimitReached = "You can not add more picture about your car. You reached limit of the car images (can only upload 5 images to a car)";
    public const string CarImageNotFound = "Car image not found";
    public const string CarImageInvalidFile = "Only PNG and JPEG images (.png, .jpg, .jpeg) can be uploaded";
}
