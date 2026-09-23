using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace WebAPI.Common;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder builder);
}
