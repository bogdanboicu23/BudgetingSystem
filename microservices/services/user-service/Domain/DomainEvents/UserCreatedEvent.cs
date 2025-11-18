namespace UserService.Domain.DomainEvents;

public class UserCreatedEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public DateTime CreatedAt { get; }

    public UserCreatedEvent(Guid userId, string email, string fullName, DateTime createdAt)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        CreatedAt = createdAt;
    }
}