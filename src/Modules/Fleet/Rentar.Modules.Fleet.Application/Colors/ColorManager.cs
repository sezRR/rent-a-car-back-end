using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Exceptions;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Colors;

public class ColorManager(IColorDal colorDal) : IColorService
{
    private readonly IColorDal _colorDal = colorDal;

    [ValidationAspect(typeof(ColorValidator))]
    public void Add(Color color)
    {
        _colorDal.Add(color);
    }

    public void Update(Color color)
    {
        EnsureNameIsValid(color);
        _colorDal.Update(color);
    }

    public void Delete(Color color)
    {
        EnsureNameIsValid(color);
        _colorDal.Delete(color);
    }

    public List<Color> GetAll()
    {
        return _colorDal.GetAll();
    }

    public Color GetById(int colorId)
    {
        return _colorDal.Get(b => b.Id == colorId);
    }

    private static void EnsureNameIsValid(Color color)
    {
        if (color.ColorName.Length < 2)
        {
            throw new BusinessRuleException(Messages.ColorNameInvalid);
        }
    }
}
