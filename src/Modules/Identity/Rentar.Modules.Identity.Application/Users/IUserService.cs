using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Kernel.Results;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

public interface IUserService
{
    IDataResult<List<User>> GetAll();

    List<OperationClaim> GetClaims(User user);
    User GetByMail(string email);

    IDataResult<User> GetUserByMail(string email);
    IDataResult<User> GetUserById(int id);

    IResult Add(User user);
    IResult Update(UserForUpdateDto userForUpdateDto);
    IResult Delete(User user);

    IDataResult<List<User>> GetUserFindeksRating(int findeksRating);
}
