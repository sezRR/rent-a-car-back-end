using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Cars;

public sealed record UpdateCar(
    int Id,
    int BrandId,
    int ColorId,
    int ModelYear,
    decimal DailyPrice,
    string Description,
    int MinimumFindeksRating);

public sealed class UpdateCarHandler(ICarDal carDal) : ICommandHandler<UpdateCar>
{
    public void Handle(UpdateCar command)
    {
        carDal.Update(new Car
        {
            Id = command.Id,
            BrandId = command.BrandId,
            ColorId = command.ColorId,
            ModelYear = command.ModelYear,
            DailyPrice = command.DailyPrice,
            Description = command.Description,
            MinimumFindeksRating = command.MinimumFindeksRating
        });
    }
}
