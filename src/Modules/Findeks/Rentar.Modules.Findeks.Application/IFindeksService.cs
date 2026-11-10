using Rentar.Shared.Kernel.Results;

namespace Rentar.Modules.Findeks.Application;

public interface IFindeksService
{
    IDataResult<int> CalculateFindeksRating();
}
