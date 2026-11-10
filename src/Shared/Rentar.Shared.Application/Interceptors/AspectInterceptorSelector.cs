using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;

namespace Rentar.Shared.Application.Interceptors;

/// <summary>
/// Runs the aspect attributes declared on the implementation class and on the invoked method, ordered by priority.
/// </summary>
public class AspectInterceptorSelector : IInterceptorSelector
{
    public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
    {
        var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();
        var methodAttributes = type.GetMethod(method.Name)
            .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
        classAttributes.AddRange(methodAttributes);

        return classAttributes.OrderBy(x => x.Priority).ToArray();
    }
}
