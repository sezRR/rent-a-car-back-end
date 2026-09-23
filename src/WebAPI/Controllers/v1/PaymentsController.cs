using Asp.Versioning;
using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class PaymentsController : ControllerBase
{
    private IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("pay")]
    public IActionResult Pay()
    {
        var result = _paymentService.Pay();

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}