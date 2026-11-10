using FluentValidation;
using Rentar.Modules.Identity.Application.Security;
using Rentar.Modules.Identity.Application.Users;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;
using Rentar.Shared.Kernel.Exceptions;

namespace Rentar.Modules.Identity.Application.Auth;

/// <summary>
/// Creates an account with a salted hash of <see cref="Password"/> and answers the stored user. Throws
/// <see cref="ConflictException"/> when the email is already registered.
/// </summary>
public sealed record RegisterUser(string FirstName, string LastName, string Email, string Password, int FindeksRating);

public sealed class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    public RegisterUserValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty();
        RuleFor(u => u.LastName).NotEmpty();
        RuleFor(u => u.Email).NotEmpty();
    }
}

public sealed class RegisterUserHandler(IUserDal userDal) : ICommandHandler<RegisterUser, User>
{
    [ValidationAspect(typeof(RegisterUserValidator))]
    public User Handle(RegisterUser command)
    {
        if (userDal.Get(u => u.Email == command.Email) != null)
        {
            throw new ConflictException(Messages.UserAlreadyExists);
        }

        HashingHelper.CreatePasswordHash(command.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Status = true,
            FindeksRating = command.FindeksRating,
        };

        userDal.Add(user);
        return user;
    }
}
