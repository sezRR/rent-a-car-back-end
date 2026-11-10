using FluentValidation;
using Rentar.Modules.Fleet.Domain;

namespace Rentar.Modules.Fleet.Application.Cars;

public class CarValidator : AbstractValidator<Car>
{
    public CarValidator()
    {
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.ModelYear).NotEmpty();
        RuleFor(c => c.Description).MinimumLength(2);
        RuleFor(c => c.DailyPrice).GreaterThan(0);
    }
}
