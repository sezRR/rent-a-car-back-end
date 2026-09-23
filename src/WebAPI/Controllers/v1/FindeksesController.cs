using Asp.Versioning;
using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FindeksesController : ControllerBase
{
    private IFindeksService _findeksService;

    public FindeksesController(IFindeksService findeksService)
    {
        _findeksService = findeksService;
    }

    [HttpGet("calculate")]
    public ActionResult CalculateFindeksRating()
    {
        var result = _findeksService.CalculateFindeksRating();

        if (result.Success) return Ok(result);

        return BadRequest(result);
    }
}