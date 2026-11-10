using FluentValidation;
using Rentar.Modules.Customers.Domain;

namespace Rentar.Modules.Customers.Application;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty();
        RuleFor(c => c.CompanyName).MinimumLength(5);
    }
}
