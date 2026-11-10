using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Colors;

/// <summary>Answers the color, or <c>null</c> when there is none with that id.</summary>
public sealed record GetColorById(int Id);

public sealed class GetColorByIdHandler(IColorDal colorDal) : IQueryHandler<GetColorById, Color>
{
    public Color Handle(GetColorById query)
    {
        return colorDal.Get(c => c.Id == query.Id);
    }
}
