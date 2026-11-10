using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Identity.Application.Auth;

public class UserForLoginDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
