#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Rentar.Modules.Fleet.Application.Brands;
using Rentar.Modules.Fleet.Domain;
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

    private static Results<Ok<List<Brand>>, ProblemHttpResult> GetAll(IBrandService brandService)
    {
        var result = brandService.GetAll();
        return result.Success ? TypedResults.Ok(result.Data) : RestResults.Problem(result);
    }

    private static Results<Ok<Brand>, ProblemHttpResult> GetById(IBrandService brandService, int id)
    {
        var brand = brandService.GetById(id).Data;
        return brand is null ? RestResults.NotFound("Brand", id) : TypedResults.Ok(brand);
    }

    private static Results<Created<Brand>, ProblemHttpResult> Create(
        IBrandService brandService,
        HttpRequest request,
        BrandRequest body)
    {
        var brand = new Brand { BrandName = body.BrandName };

        var result = brandService.Add(brand);
        return result.Success ? RestResults.Created(request, brand.Id, brand) : RestResults.Problem(result);
    }

    private static Results<NoContent, ProblemHttpResult> Update(IBrandService brandService, int id, BrandRequest body)
    {
        if (brandService.GetById(id).Data is null)
        {
            return RestResults.NotFound("Brand", id);
        }

        var result = brandService.Update(new Brand { Id = id, BrandName = body.BrandName });
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }

    private static Results<NoContent, ProblemHttpResult> Delete(IBrandService brandService, int id)
    {
        var brand = brandService.GetById(id).Data;
        if (brand is null)
        {
            return RestResults.NotFound("Brand", id);
        }

        var result = brandService.Delete(brand);
        return result.Success ? TypedResults.NoContent() : RestResults.Problem(result);
    }
}
