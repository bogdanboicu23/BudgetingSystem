using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Handlers;
using UserService.Application.Queries;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CreateUserCommandHandler _createUserHandler;
    private readonly GetUserQueryHandler _getUserHandler;

    public UsersController(
        CreateUserCommandHandler createUserHandler,
        GetUserQueryHandler getUserHandler)
    {
        _createUserHandler = createUserHandler;
        _getUserHandler = getUserHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var query = new GetAllUsersQuery();
        var users = await _getUserHandler.Handle(query);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var query = new GetUserQuery { UserId = id };
        var user = await _getUserHandler.Handle(query);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var query = new GetUserByEmailQuery { Email = email };
        var user = await _getUserHandler.Handle(query);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            var command = new CreateUserCommand
            {
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Password = createUserDto.Password
            };

            var user = await _createUserHandler.Handle(command);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}