using FluentValidation;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Application.Aspects.Validation;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Identity.Application.Users;

/// <summary>
/// Stores a user exactly as given, hash and salt included. Accounts with a password are created by
/// <see cref="Auth.RegisterUser"/>.
/// </summary>
public sealed record AddUser(
    string FirstName,
    string LastName,
    string Email,
    byte[] PasswordHash,
    byte[] PasswordSalt,
    bool Status,
    int FindeksRating);

public sealed class AddUserValidator : AbstractValidator<AddUser>
{
    public AddUserValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty();
        RuleFor(u => u.LastName).NotEmpty();
        RuleFor(u => u.Email).NotEmpty();
    }
}

public sealed class AddUserHandler(IUserDal userDal) : ICommandHandler<AddUser>
{
    [ValidationAspect(typeof(AddUserValidator))]
    public void Handle(AddUser command)
    {
        userDal.Add(new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PasswordHash = command.PasswordHash,
            PasswordSalt = command.PasswordSalt,
            Status = command.Status,
            FindeksRating = command.FindeksRating
        });
    }
}
