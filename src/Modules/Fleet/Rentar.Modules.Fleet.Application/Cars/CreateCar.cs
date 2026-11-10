using FluentValidation;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Application.Aspects.Caching;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Fleet.Application.Cars;

/// <summary>Stores a new car and answers it with its id set.</summary>
public sealed record CreateCar(
    int BrandId,
    int ColorId,
    int ModelYear,
    decimal DailyPrice,
    string Description,
    int MinimumFindeksRating);

public sealed class CreateCarValidator : AbstractValidator<CreateCar>
{
    public CreateCarValidator()
    {
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.ModelYear).NotEmpty();
        RuleFor(c => c.Description).MinimumLength(2);
        RuleFor(c => c.DailyPrice).GreaterThan(0);
    }
}

public sealed class CreateCarHandler(ICarDal carDal) : ICommandHandler<CreateCar, Car>
{
    /// <summary>
    /// Matches the cache keys of the car queries, e.g. <see cref="GetCarsByBrandHandler"/>, so a new car shows up at once.
    /// </summary>
    private const string CarQueries = @"Rentar\.Modules\.Fleet\.Application\.Cars\.Get";

    [ValidationAspect(typeof(CreateCarValidator))]
    [CacheRemoveAspect(CarQueries)]
    public Car Handle(CreateCar command)
    {
        var car = new Car
        {
            BrandId = command.BrandId,
            ColorId = command.ColorId,
            ModelYear = command.ModelYear,
            DailyPrice = command.DailyPrice,
            Description = command.Description,
            MinimumFindeksRating = command.MinimumFindeksRating
        };

        carDal.Add(car);
        return car;
    }
}
