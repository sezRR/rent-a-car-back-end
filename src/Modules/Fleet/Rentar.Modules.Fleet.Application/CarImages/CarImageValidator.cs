using FluentValidation;
using Rentar.Modules.Fleet.Domain;

namespace Rentar.Modules.Fleet.Application.CarImages;

public class CarImageValidator : AbstractValidator<CarImage>
{
    public CarImageValidator()
    {
        RuleFor(c => c.ImagePath).NotEmpty();
        RuleFor(c => c.CarId).NotEmpty();
    }
}
