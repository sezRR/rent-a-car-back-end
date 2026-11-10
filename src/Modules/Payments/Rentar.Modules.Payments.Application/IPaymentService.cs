namespace Rentar.Modules.Payments.Application;

public interface IPaymentService
{
    /// <returns><c>true</c> when the payment went through, <c>false</c> when it was declined.</returns>
    bool Pay();
}
