namespace UserService.Application.Queries;

public class GetUserQuery
{
    public Guid UserId { get; set; }
}

public class GetUserByEmailQuery
{
    public string Email { get; set; } = string.Empty;
}

public class GetAllUsersQuery
{
}