using FluentValidation;
using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Customers.Application;

/// <summary>Stores a new customer of the Identity user <see cref="UserId"/> and answers it with its id set.</summary>
public sealed record CreateCustomer(int UserId, string CompanyName);

public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomer>
{
    public CreateCustomerValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty();
        RuleFor(c => c.CompanyName).MinimumLength(5);
    }
}

public sealed class CreateCustomerHandler(ICustomerDal customerDal) : ICommandHandler<CreateCustomer, Customer>
{
    [ValidationAspect(typeof(CreateCustomerValidator))]
    public Customer Handle(CreateCustomer command)
    {
        var customer = new Customer { UserId = command.UserId, CompanyName = command.CompanyName };
        customerDal.Add(customer);
        return customer;
    }
}
