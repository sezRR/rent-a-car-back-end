using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Results;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Application;

public class CustomerManager(ICustomerDal customerDal, TimeProvider timeProvider) : ICustomerService
{
    private readonly ICustomerDal _customerDal = customerDal;
    private readonly TimeProvider _timeProvider = timeProvider;

    [ValidationAspect(typeof(CustomerValidator))]
    public IResult Add(Customer customer)
    {
        _customerDal.Add(customer);

        return new SuccessResult(Messages.CustomerAdded);
    }

    public IResult Delete(Customer customer)
    {
        if (customer.CompanyName.Length < 2)
        {
            return new ErrorResult(Messages.CustomerCompanyNameInvalid);
        }
        _customerDal.Delete(customer);

        return new SuccessResult(Messages.CustomerDeleted);
    }

    public IDataResult<List<Customer>> GetAll()
    {
        if (_timeProvider.GetLocalNow().Hour == 22)
        {
            return new ErrorDataResult<List<Customer>>(Messages.MaintenanceTime);
        }

        return new SuccessDataResult<List<Customer>>(_customerDal.GetAll(), Messages.CustomersListed);
    }

    public IDataResult<Customer> GetById(int id)
    {
        return new SuccessDataResult<Customer>(_customerDal.Get(c => c.Id == id));
    }

    public IDataResult<List<Customer>> GetCustomersByUserId(int userId)
    {
        return new SuccessDataResult<List<Customer>>(_customerDal.GetAll(c => c.UserId == userId));
    }

    public IResult Update(Customer customer)
    {
        if (customer.CompanyName.Length < 2)
        {
            return new ErrorResult(Messages.CustomerCompanyNameInvalid);
        }
        _customerDal.Update(customer);

        return new SuccessResult(Messages.CustomerUpdated);
    }
}
