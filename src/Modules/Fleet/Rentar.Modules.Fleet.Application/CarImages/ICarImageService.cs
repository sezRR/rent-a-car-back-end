using Rentar.Modules.Fleet.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.CarImages;

public interface ICarImageService
{
    List<CarImage> GetAll();

    /// <returns>The image, or <c>null</c> when there is none with that id.</returns>
    CarImage GetById(int id);

    List<CarImage> GetByCarId(int carId);

    /// <summary>Stores the file, or the default image when there is none, and sets the image's id and path.</summary>
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The car has reached its image limit, or the file is not a PNG or JPEG.</exception>
    void Add(CarImage carImage, FileUpload file);

    /// <summary>Replaces the stored image's file with <paramref name="file"/>.</summary>
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The file is missing or not a PNG or JPEG.</exception>
    /// <exception cref="Rentar.Shared.Kernel.Exceptions.NotFoundException">There is no image with that id.</exception>
    void Update(CarImage carImage, FileUpload file);

    void Delete(CarImage carImage);
}
