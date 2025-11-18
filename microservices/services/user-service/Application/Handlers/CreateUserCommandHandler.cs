using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers;

public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(CreateUserCommand command)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(command.Email))
        {
            throw new InvalidOperationException($"User with email {command.Email} already exists");
        }

        // Hash password (in real implementation, use proper password hashing)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Create user
        var user = new User(command.Email, command.FirstName, command.LastName, passwordHash);

        // Save user
        var createdUser = await _userRepository.AddAsync(user);

        // Return DTO
        return new UserDto
        {
            Id = createdUser.Id,
            Email = createdUser.Email,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            FullName = createdUser.GetFullName(),
            CreatedAt = createdUser.CreatedAt,
            UpdatedAt = createdUser.UpdatedAt,
            IsActive = createdUser.IsActive
        };
    }
}