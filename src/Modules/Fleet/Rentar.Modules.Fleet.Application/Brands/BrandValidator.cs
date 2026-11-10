using FluentValidation;
using Rentar.Modules.Fleet.Domain;

namespace Rentar.Modules.Fleet.Application.Brands;

public class BrandValidator : AbstractValidator<Brand>
{
    public BrandValidator()
    {
        RuleFor(b => b.BrandName).NotEmpty();
        RuleFor(b => b.BrandName).MinimumLength(2);
    }
}
