using Rentar.Modules.Fleet.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Brands;

public interface IBrandService
{
    List<Brand> GetAll();

    /// <returns>The brand, or <c>null</c> when there is none with that id.</returns>
    Brand GetById(int brandId);

    void Add(Brand brand);
    void Update(Brand brand);
    void Delete(Brand brand);
}
