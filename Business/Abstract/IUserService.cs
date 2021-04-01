using Core.Utilities.Results;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Concrete;
using User = Core.Entities.Concrete.User;
using Entities.DTOs;

namespace Business.Abstract
{
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
}
