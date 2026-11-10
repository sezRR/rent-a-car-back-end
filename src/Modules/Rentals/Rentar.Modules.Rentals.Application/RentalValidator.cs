using FluentValidation;
using Rentar.Modules.Rentals.Domain;

namespace Rentar.Modules.Rentals.Application;

public class RentalValidator : AbstractValidator<Rental>
{
    public RentalValidator()
    {
        RuleFor(r => r.RentDate).NotEmpty();
        RuleFor(r => r.ReturnDate).NotEmpty();
    }
}
