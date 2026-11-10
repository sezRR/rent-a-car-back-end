using Rentar.Modules.Identity.Domain;
using System.Collections.Generic;

namespace Rentar.Modules.Identity.Application.Auth;

public interface ITokenHelper
{
    AccessToken CreateToken(User user, List<OperationClaim> operationClaims);
}
