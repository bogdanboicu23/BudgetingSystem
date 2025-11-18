namespace UserService.Application.Commands;

public class UpdateUserCommand
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}