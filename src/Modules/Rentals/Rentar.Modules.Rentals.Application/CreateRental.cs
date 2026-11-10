using FluentValidation;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;
using System;

namespace Rentar.Modules.Rentals.Application;

/// <summary>
/// Stores a new rental and answers it with its id set. It does not check that the car is free; ask
/// <see cref="CheckCarAvailability"/> first.
/// </summary>
public sealed record CreateRental(int CarId, int CustomerId, DateTime RentDate, DateTime ReturnDate);

public sealed class CreateRentalValidator : AbstractValidator<CreateRental>
{
    public CreateRentalValidator()
    {
        RuleFor(r => r.RentDate).NotEmpty();
        RuleFor(r => r.ReturnDate).NotEmpty();
    }
}

public sealed class CreateRentalHandler(IRentalDal rentalDal) : ICommandHandler<CreateRental, Rental>
{
    [ValidationAspect(typeof(CreateRentalValidator))]
    public Rental Handle(CreateRental command)
    {
        var rental = new Rental
        {
            CarId = command.CarId,
            CustomerId = command.CustomerId,
            RentDate = command.RentDate,
            ReturnDate = command.ReturnDate
        };

        rentalDal.Add(rental);
        return rental;
    }
}
