using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Application;

public class CustomerManager(ICustomerDal customerDal, TimeProvider timeProvider) : ICustomerService
{
    private readonly ICustomerDal _customerDal = customerDal;
    private readonly TimeProvider _timeProvider = timeProvider;

    [ValidationAspect(typeof(CustomerValidator))]
    public void Add(Customer customer)
    {
        _customerDal.Add(customer);
    }

    public void Delete(Customer customer)
    {
        EnsureCompanyNameIsValid(customer);
        _customerDal.Delete(customer);
    }

    public List<Customer> GetAll()
    {
        if (_timeProvider.GetLocalNow().Hour == 22)
        {
            throw new UnavailableException(Messages.MaintenanceTime);
        }

        return _customerDal.GetAll();
    }

    public Customer GetById(int id)
    {
        return _customerDal.Get(c => c.Id == id);
    }

    public List<Customer> GetCustomersByUserId(int userId)
    {
        return _customerDal.GetAll(c => c.UserId == userId);
    }

    public void Update(Customer customer)
    {
        EnsureCompanyNameIsValid(customer);
        _customerDal.Update(customer);
    }

    private static void EnsureCompanyNameIsValid(Customer customer)
    {
        if (customer.CompanyName.Length < 2)
        {
            throw new BusinessRuleException(Messages.CustomerCompanyNameInvalid);
        }
    }
}
