using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace Rentar.Shared.Presentation.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder builder);
}
