using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Brands;

public sealed record UpdateBrand(int Id, string BrandName);

public sealed class UpdateBrandHandler(IBrandDal brandDal) : ICommandHandler<UpdateBrand>
{
    public void Handle(UpdateBrand command)
    {
        brandDal.Update(new Brand { Id = command.Id, BrandName = command.BrandName });
    }
}
