using Rentar.Modules.Payments.Contracts;

namespace Rentar.Modules.Payments.Application;

public class PaymentsModuleApi(IPaymentService paymentService) : IPaymentsModuleApi
{
    private readonly IPaymentService _paymentService = paymentService;

    public bool Pay()
    {
        return _paymentService.Pay();
    }
}
