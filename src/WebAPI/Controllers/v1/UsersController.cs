using Asp.Versioning;
using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using User = Core.Entities.Concrete.User;

namespace WebAPI.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        var result = _userService.GetAll();
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("getbymail")]
    public IActionResult GetByMail(string email)
    {
        var result = _userService.GetUserByMail(email);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost("add")]
    public IActionResult Add(User user)
    {
        var result = _userService.Add(user);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost("update")]
    public IActionResult Update(UserForUpdateDto userForUpdateDto)
    {
        var result = _userService.Update(userForUpdateDto);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("getuserfindeksrating")]
    public IActionResult GetUserFindeksRating(int findeksRating)
    {
        var result = _userService.GetUserFindeksRating(findeksRating);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("getuserbyid")]
    public IActionResult GetUserById(int id)
    {
        var result = _userService.GetUserById(id);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}