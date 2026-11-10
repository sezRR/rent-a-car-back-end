#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Brands;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Presentation.Endpoints;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Presentation.Endpoints.V2;

public class BrandsEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapVersionedGroup("Brands", "brands", 2.0);

        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById)
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create)
            .ProducesValidationProblem();
        group.MapPut("/{id:int}", Update)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{id:int}", Delete)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static Ok<List<Brand>> GetAll(IQueryHandler<GetBrands, List<Brand>> getBrands)
    {
        return TypedResults.Ok(getBrands.Handle(new GetBrands()));
    }

    private static Results<Ok<Brand>, ProblemHttpResult> GetById(IQueryHandler<GetBrandById, Brand> getBrandById, int id)
    {
        var brand = getBrandById.Handle(new GetBrandById(id));
        return brand is null ? RestResults.NotFound("Brand", id) : TypedResults.Ok(brand);
    }

    private static Created<Brand> Create(ICommandHandler<CreateBrand, Brand> createBrand, HttpRequest request, BrandRequest body)
    {
        var brand = createBrand.Handle(new CreateBrand(body.BrandName));
        return RestResults.Created(request, brand.Id, brand);
    }

    private static Results<NoContent, ProblemHttpResult> Update(
        IQueryHandler<GetBrandById, Brand> getBrandById,
        ICommandHandler<UpdateBrand> updateBrand,
        int id,
        BrandRequest body)
    {
        if (getBrandById.Handle(new GetBrandById(id)) is null)
        {
            return RestResults.NotFound("Brand", id);
        }

        updateBrand.Handle(new UpdateBrand(id, body.BrandName));
        return TypedResults.NoContent();
    }

    private static Results<NoContent, ProblemHttpResult> Delete(
        IQueryHandler<GetBrandById, Brand> getBrandById,
        ICommandHandler<DeleteBrand> deleteBrand,
        int id)
    {
        if (getBrandById.Handle(new GetBrandById(id)) is null)
        {
            return RestResults.NotFound("Brand", id);
        }

        deleteBrand.Handle(new DeleteBrand(id));
        return TypedResults.NoContent();
    }
}
