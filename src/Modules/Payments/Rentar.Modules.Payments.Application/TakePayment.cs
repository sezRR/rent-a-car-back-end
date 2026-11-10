using Rentar.Shared.Application.UseCases;
using System;

namespace Rentar.Modules.Payments.Application;

/// <summary>Takes a payment and answers <c>true</c> when it went through, <c>false</c> when it was declined.</summary>
public sealed record TakePayment;

public sealed class TakePaymentHandler : ICommandHandler<TakePayment, bool>
{
    public bool Handle(TakePayment command)
    {
        // No card data is kept, so the outcome of a payment is simulated.
        return Random.Shared.Next(2) == 1;
    }
}
