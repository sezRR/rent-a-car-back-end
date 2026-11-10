using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Kernel.Exceptions;
using System;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Users;

public class UserManager(IUserDal userDal, TimeProvider timeProvider) : IUserService
{
    private readonly IUserDal _userDal = userDal;
    private readonly TimeProvider _timeProvider = timeProvider;

    [ValidationAspect(typeof(UserValidator))]
    public void Add(User user)
    {
        _userDal.Add(user);
    }

    public void Delete(User user)
    {
        if (user.FirstName.Length < 2)
        {
            throw new BusinessRuleException(Messages.UserFirstNameInvalid);
        }

        _userDal.Delete(user);
    }

    public List<User> GetAll()
    {
        if (_timeProvider.GetLocalNow().Hour == 22)
        {
            throw new UnavailableException(Messages.MaintenanceTime);
        }

        return _userDal.GetAll();
    }

    public User GetByMail(string email)
    {
        return _userDal.Get(u => u.Email == email);
    }

    public List<OperationClaim> GetClaims(User user)
    {
        return _userDal.GetClaims(user);
    }

    public User Update(UserForUpdateDto userForUpdateDto)
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
        return user;
    }

    public List<User> GetUserFindeksRating(int findeksRating)
    {
        return _userDal.GetAll(u => u.FindeksRating >= findeksRating);
    }

    public User GetById(int id)
    {
        return _userDal.Get(u => u.Id == id);
    }
}
