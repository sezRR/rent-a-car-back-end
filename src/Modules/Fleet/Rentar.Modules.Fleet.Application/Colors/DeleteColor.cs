using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Colors;

public sealed record DeleteColor(int Id);

public sealed class DeleteColorHandler(IColorDal colorDal) : ICommandHandler<DeleteColor>
{
    public void Handle(DeleteColor command)
    {
        colorDal.Delete(new Color { Id = command.Id });
    }
}
