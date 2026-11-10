using FluentValidation;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Brands;

/// <summary>Stores a new brand and answers it with its id set.</summary>
public sealed record CreateBrand(string BrandName);

public sealed class CreateBrandValidator : AbstractValidator<CreateBrand>
{
    public CreateBrandValidator()
    {
        RuleFor(b => b.BrandName).NotEmpty();
        RuleFor(b => b.BrandName).MinimumLength(2);
    }
}

public sealed class CreateBrandHandler(IBrandDal brandDal) : ICommandHandler<CreateBrand, Brand>
{
    [ValidationAspect(typeof(CreateBrandValidator))]
    public Brand Handle(CreateBrand command)
    {
        var brand = new Brand { BrandName = command.BrandName };
        brandDal.Add(brand);
        return brand;
    }
}
