using Rentar.Modules.Findeks.Contracts;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Findeks.Application;

public class FindeksModuleApi(IQueryHandler<CalculateFindeksRating, int> calculateFindeksRating) : IFindeksModuleApi
{
    public int CalculateFindeksRating()
    {
        return calculateFindeksRating.Handle(new CalculateFindeksRating());
    }
}
