using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Colors;

public sealed record GetColors;

public sealed class GetColorsHandler(IColorDal colorDal) : IQueryHandler<GetColors, List<Color>>
{
    public List<Color> Handle(GetColors query)
    {
        return colorDal.GetAll();
    }
}
