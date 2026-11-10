using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Brands;

public sealed record DeleteBrand(int Id);

public sealed class DeleteBrandHandler(IBrandDal brandDal) : ICommandHandler<DeleteBrand>
{
    public void Handle(DeleteBrand command)
    {
        brandDal.Delete(new Brand { Id = command.Id });
    }
}
