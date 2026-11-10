using Castle.DynamicProxy;
using Rentar.Shared.Application.Interceptors;
using System.Transactions;

namespace Rentar.Shared.Application.Aspects.Transaction;

public class TransactionScopeAspect : MethodInterception
{
    public override void Intercept(IInvocation invocation)
    {
        using var transactionScope = new TransactionScope();

        invocation.Proceed();
        transactionScope.Complete();
    }
}
