using FluentValidation;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Colors;

/// <summary>Stores a new color and answers it with its id set.</summary>
public sealed record CreateColor(string ColorName);

public sealed class CreateColorValidator : AbstractValidator<CreateColor>
{
    public CreateColorValidator()
    {
        RuleFor(c => c.ColorName).NotEmpty();
        RuleFor(c => c.ColorName).MinimumLength(2);
    }
}

public sealed class CreateColorHandler(IColorDal colorDal) : ICommandHandler<CreateColor, Color>
{
    [ValidationAspect(typeof(CreateColorValidator))]
    public Color Handle(CreateColor command)
    {
        var color = new Color { ColorName = command.ColorName };
        colorDal.Add(color);
        return color;
    }
}
