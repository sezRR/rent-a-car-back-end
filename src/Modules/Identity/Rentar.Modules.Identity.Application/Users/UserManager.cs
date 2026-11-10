using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Results;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

public class UserManager(IUserDal userDal, TimeProvider timeProvider) : IUserService
{
    private readonly IUserDal _userDal = userDal;
    private readonly TimeProvider _timeProvider = timeProvider;

    [ValidationAspect(typeof(UserValidator))]
    public IResult Add(User user)
    {
        _userDal.Add(user);

        return new SuccessResult(Messages.UserAdded);
    }

    public IResult Delete(User user)
    {
        if (user.FirstName.Length < 2)
        {
            return new ErrorResult(Messages.UserFirstNameInvalid);
        }
        _userDal.Delete(user);

        return new SuccessResult(Messages.UserDeleted);
    }

    public IDataResult<List<User>> GetAll()
    {
        if (_timeProvider.GetLocalNow().Hour == 22)
        {
            return new ErrorDataResult<List<User>>(Messages.MaintenanceTime);
        }

        return new SuccessDataResult<List<User>>(_userDal.GetAll(), Messages.UsersListed);
    }

    public User GetByMail(string email)
    {
        return _userDal.Get(u => u.Email == email);
    }

    public List<OperationClaim> GetClaims(User user)
    {
        return _userDal.GetClaims(user);
    }

    public IDataResult<User> GetUserByMail(string email)
    {
        return new SuccessDataResult<User>(_userDal.Get(e => e.Email == email));
    }

    public IResult Update(UserForUpdateDto userForUpdateDto)
    {
        HashingHelper.CreatePasswordHash(userForUpdateDto.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Id = userForUpdateDto.Id,
            FirstName = userForUpdateDto.FirstName,
            LastName = userForUpdateDto.LastName,
            Email = userForUpdateDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Status = true,
            FindeksRating = userForUpdateDto.FindeksRating,
        };

        _userDal.Update(user);
        return new SuccessDataResult<User>(user, Messages.UserUpdated);
    }

    public IDataResult<List<User>> GetUserFindeksRating(int findeksRating)
    {
        return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.FindeksRating >= findeksRating));
    }

    public IDataResult<User> GetUserById(int id)
    {
        return new SuccessDataResult<User>(_userDal.Get(u => u.Id == id));
    }
}
