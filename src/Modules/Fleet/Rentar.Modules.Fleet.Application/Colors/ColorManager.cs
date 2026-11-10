using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Colors;

public class ColorManager(IColorDal colorDal) : IColorService
{
    private readonly IColorDal _colorDal = colorDal;

    [ValidationAspect(typeof(ColorValidator))]
    public IResult Add(Color color)
    {
        _colorDal.Add(color);

        return new SuccessResult(Messages.ColorAdded);
    }

    public IResult Update(Color color)
    {
        if (color.ColorName.Length < 2)
        {
            return new ErrorResult(Messages.ColorNameInvalid);
        }
        _colorDal.Update(color);

        return new SuccessResult(Messages.ColorUpdated);
    }

    public IResult Delete(Color color)
    {
        if (color.ColorName.Length < 2)
        {
            return new ErrorResult(Messages.ColorNameInvalid);
        }
        _colorDal.Delete(color);

        return new SuccessResult(Messages.ColorDeleted);
    }

    public IDataResult<List<Color>> GetAll()
    {
        return new SuccessDataResult<List<Color>>(_colorDal.GetAll(), Messages.ColorsListed);
    }

    public IDataResult<Color> GetById(int colorId)
    {
        return new SuccessDataResult<Color>(_colorDal.Get(b => b.Id == colorId));
    }
}
