using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation;

public class CarImageValidator : AbstractValidator<CarImage>
{
    public CarImageValidator()
    {
        RuleFor(c => c.ImagePath).NotEmpty();
        RuleFor(c => c.CarId).NotEmpty();
    }
}
