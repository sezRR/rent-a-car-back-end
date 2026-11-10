using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Customers.Application;

public interface ICustomerService
{
    IDataResult<List<Customer>> GetAll();
    IDataResult<Customer> GetById(int id);
    IDataResult<List<Customer>> GetCustomersByUserId(int userId);

    IResult Add(Customer customer);
    IResult Update(Customer customer);
    IResult Delete(Customer customer);
}
