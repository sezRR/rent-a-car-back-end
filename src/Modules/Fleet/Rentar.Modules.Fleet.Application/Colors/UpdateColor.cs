using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;

namespace Rentar.Modules.Fleet.Application.Colors;

/// <summary>Replaces the color; throws <see cref="BusinessRuleException"/> when the name is shorter than two characters.</summary>
public sealed record UpdateColor(int Id, string ColorName);

public sealed class UpdateColorHandler(IColorDal colorDal) : ICommandHandler<UpdateColor>
{
    public void Handle(UpdateColor command)
    {
        if (command.ColorName is not { Length: >= 2 })
        {
            throw new BusinessRuleException(Messages.ColorNameInvalid);
        }

        colorDal.Update(new Color { Id = command.Id, ColorName = command.ColorName });
    }
}
