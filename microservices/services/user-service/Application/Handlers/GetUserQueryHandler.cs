using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Repositories;

namespace UserService.Application.Handlers;

public class GetUserQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserQuery query)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }

    public async Task<UserDto?> Handle(GetUserByEmailQuery query)
    {
        var user = await _userRepository.GetByEmailAsync(query.Email);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery query)
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        });
    }
}