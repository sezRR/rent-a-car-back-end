using FluentValidation;
using Rentar.Modules.Fleet.Domain;

namespace Rentar.Modules.Fleet.Application.Colors;

public class ColorValidator : AbstractValidator<Color>
{
    public ColorValidator()
    {
        RuleFor(c => c.ColorName).NotEmpty();
        RuleFor(c => c.ColorName).MinimumLength(2);
    }
}
