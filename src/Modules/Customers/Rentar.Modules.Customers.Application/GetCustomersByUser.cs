using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.UseCases;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Application;

/// <summary>The customers that belong to the Identity user <see cref="UserId"/>.</summary>
public sealed record GetCustomersByUser(int UserId);

public sealed class GetCustomersByUserHandler(ICustomerDal customerDal) : IQueryHandler<GetCustomersByUser, List<Customer>>
{
    public List<Customer> Handle(GetCustomersByUser query)
    {
        return customerDal.GetAll(c => c.UserId == query.UserId);
    }
}
