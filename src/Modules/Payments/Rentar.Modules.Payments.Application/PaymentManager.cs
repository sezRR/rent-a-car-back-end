using Rentar.Shared.Kernel.Results;
using System;

namespace Rentar.Modules.Payments.Application;

public class PaymentManager : IPaymentService
{
    public IResult Pay()
    {
        // No card data is kept, so the outcome of a payment is simulated.
        return Random.Shared.Next(2) == 1
            ? new SuccessResult(Messages.PaymentSuccessful)
            : new ErrorResult(Messages.PaymentFailed);
    }
}
