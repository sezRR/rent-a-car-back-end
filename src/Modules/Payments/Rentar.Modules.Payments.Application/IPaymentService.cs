using Rentar.Shared.Kernel.Results;

namespace Rentar.Modules.Payments.Application;

public interface IPaymentService
{
    IResult Pay();
}
