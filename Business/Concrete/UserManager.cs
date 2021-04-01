using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using Core.Utilities.Security.Hashing;
using Entities.DTOs;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;
        IFindeksService _findeksService;

        public UserManager(IUserDal userDal, IFindeksService findeksService)
        {
            _userDal = userDal;
            _findeksService = findeksService;
        }

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
            if (DateTime.Now.Hour == 22)
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
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(userForUpdateDto.Password, out passwordHash, out passwordSalt);

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
            //burda eşleşen kullanıcıları çek front endde gelen listede işlem yapmak isteyen var mı check edecek kodu yaz.
            return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.FindeksRating >= findeksRating)); 
        }

        public IDataResult<User> GetUserById(int id)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Id == id));
        }
    }
}
