using Rentar.Modules.Findeks.Contracts;

namespace Rentar.Modules.Findeks.Application;

public class FindeksModuleApi(IFindeksService findeksService) : IFindeksModuleApi
{
    private readonly IFindeksService _findeksService = findeksService;

    public int CalculateFindeksRating()
    {
        return _findeksService.CalculateFindeksRating().Data;
    }
}
