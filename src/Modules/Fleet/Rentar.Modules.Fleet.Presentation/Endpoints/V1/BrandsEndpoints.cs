using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application;
using Rentar.Modules.Fleet.Application.Brands;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V1;

public class BrandsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Brands", "brands", 1.0);

        group.MapGet("/getall", GetAll);
        group.MapGet("/getbyid", GetById);
        group.MapPost("/add", Add);
        group.MapPost("/update", Update);
        group.MapPost("/delete", Delete);
    }

    private static Ok<V1Response<List<Brand>>> GetAll(IQueryHandler<GetBrands, List<Brand>> getBrands)
    {
        return V1Results.Ok(getBrands.Handle(new GetBrands()), Messages.BrandsListed);
    }

    private static Ok<V1Response<Brand>> GetById(IQueryHandler<GetBrandById, Brand> getBrandById, int id)
    {
        return V1Results.Ok(getBrandById.Handle(new GetBrandById(id)), null);
    }

    private static Ok<V1Response> Add(ICommandHandler<CreateBrand, Brand> createBrand, CreateBrand command)
    {
        createBrand.Handle(command);
        return V1Results.Ok(Messages.BrandAdded);
    }

    private static Ok<V1Response> Update(ICommandHandler<UpdateBrand> updateBrand, UpdateBrand command)
    {
        updateBrand.Handle(command);
        return V1Results.Ok(Messages.BrandUpdated);
    }

    private static Ok<V1Response> Delete(ICommandHandler<DeleteBrand> deleteBrand, DeleteBrand command)
    {
        deleteBrand.Handle(command);
        return V1Results.Ok(Messages.BrandDeleted);
    }
}
