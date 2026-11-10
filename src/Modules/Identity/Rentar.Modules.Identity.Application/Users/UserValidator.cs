using FluentValidation;
using Rentar.Modules.Identity.Domain;

namespace Rentar.Modules.Identity.Application.Users;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty();
        RuleFor(u => u.LastName).NotEmpty();
        RuleFor(u => u.Email).NotEmpty();
    }
}
