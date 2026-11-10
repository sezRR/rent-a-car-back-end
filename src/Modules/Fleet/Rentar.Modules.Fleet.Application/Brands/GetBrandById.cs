using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Brands;

/// <summary>Answers the brand, or <c>null</c> when there is none with that id.</summary>
public sealed record GetBrandById(int Id);

public sealed class GetBrandByIdHandler(IBrandDal brandDal) : IQueryHandler<GetBrandById, Brand>
{
    public Brand Handle(GetBrandById query)
    {
        return brandDal.Get(b => b.Id == query.Id);
    }
}
