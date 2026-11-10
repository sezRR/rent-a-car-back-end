using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Application;

/// <summary>Every customer; throws <see cref="UnavailableException"/> during the 22:00 maintenance hour.</summary>
public sealed record GetCustomers;

public sealed class GetCustomersHandler(ICustomerDal customerDal, TimeProvider timeProvider) : IQueryHandler<GetCustomers, List<Customer>>
{
    public List<Customer> Handle(GetCustomers query)
    {
        if (timeProvider.GetLocalNow().Hour == 22)
        {
            throw new UnavailableException(Messages.MaintenanceTime);
        }

        return customerDal.GetAll();
    }
}
