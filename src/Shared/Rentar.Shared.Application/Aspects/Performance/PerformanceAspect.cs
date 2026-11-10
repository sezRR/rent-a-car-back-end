using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Rentar.Shared.Application.Interceptors;
using Rentar.Shared.Application.IoC;
using System.Diagnostics;

namespace Rentar.Shared.Application.Aspects.Performance;

public class PerformanceAspect(int interval) : MethodInterception
{
    private readonly int _interval = interval;
    private readonly Stopwatch _stopwatch = ServiceTool.ServiceProvider.GetRequiredService<Stopwatch>();

    protected override void OnBefore(IInvocation invocation)
    {
        _stopwatch.Start();
    }

    protected override void OnAfter(IInvocation invocation)
    {
        if (_stopwatch.Elapsed.TotalSeconds > _interval)
        {
            Debug.WriteLine($"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}");
        }
        _stopwatch.Reset();
    }
}
