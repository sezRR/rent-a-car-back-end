using Rentar.Modules.Fleet.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Fleet.Application.Colors;

public interface IColorService
{
    List<Color> GetAll();

    /// <returns>The color, or <c>null</c> when there is none with that id.</returns>
    Color GetById(int colorId);

    void Add(Color color);

    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The name is shorter than two characters.</exception>
    void Update(Color color);

    /// <exception cref="Rentar.Shared.Kernel.Exceptions.BusinessRuleException">The name is shorter than two characters.</exception>
    void Delete(Color color);
}
