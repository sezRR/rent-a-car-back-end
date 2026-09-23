using Asp.Versioning;
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class RentalsController : ControllerBase
{
    IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        var result = _rentalService.GetAll();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("getrentalbycarid")]
    public IActionResult GetRentalByCarId(int carId)
    {
        var result = _rentalService.GetRentalByCarId(carId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("getrentaldetails")]
    public IActionResult GetRentalDetails()
    {
        var result = _rentalService.GetRentalDetails();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost("isrentable")]
    public IActionResult IsRentable(Rental rental)
    {
        var result = _rentalService.IsRentable(rental);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost("add")]
    public IActionResult Add(Rental rental)
    {
        var result = _rentalService.Add(rental);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}