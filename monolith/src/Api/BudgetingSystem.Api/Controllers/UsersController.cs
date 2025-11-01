using Microsoft.AspNetCore.Mvc;
using BudgetingSystem.Users.Application.Services;

namespace BudgetingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request.Email, request.FirstName, request.LastName, request.Password);
        return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, user);
    }
}

public record CreateUserRequest(string Email, string FirstName, string LastName, string Password);