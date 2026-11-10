using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Customers.Application;

/// <summary>Answers the customer, or <c>null</c> when there is none with that id.</summary>
public sealed record GetCustomerById(int Id);

public sealed class GetCustomerByIdHandler(ICustomerDal customerDal) : IQueryHandler<GetCustomerById, Customer>
{
    public Customer Handle(GetCustomerById query)
    {
        return customerDal.Get(c => c.Id == query.Id);
    }
}
