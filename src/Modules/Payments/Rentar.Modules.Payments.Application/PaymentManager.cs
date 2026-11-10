using System;

namespace Rentar.Modules.Payments.Application;

public class PaymentManager : IPaymentService
{
    public bool Pay()
    {
        // No card data is kept, so the outcome of a payment is simulated.
        return Random.Shared.Next(2) == 1;
    }
}
