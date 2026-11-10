using Rentar.Modules.Payments.Contracts;
using Rentar.Shared.Application.UseCases;

namespace Rentar.Modules.Payments.Application;

public class PaymentsModuleApi(ICommandHandler<TakePayment, bool> takePayment) : IPaymentsModuleApi
{
    public bool Pay()
    {
        return takePayment.Handle(new TakePayment());
    }
}
