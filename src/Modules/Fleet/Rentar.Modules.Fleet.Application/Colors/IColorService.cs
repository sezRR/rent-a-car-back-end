using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Colors;

public interface IColorService
{
    IDataResult<List<Color>> GetAll();
    IDataResult<Color> GetById(int colorId);

    IResult Add(Color color);
    IResult Update(Color color);
    IResult Delete(Color color);
}
