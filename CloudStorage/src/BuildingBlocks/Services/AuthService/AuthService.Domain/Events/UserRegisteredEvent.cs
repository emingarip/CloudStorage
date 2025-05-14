using SharedKernel;

namespace AuthService.Domain.Events;

public class UserRegisteredEvent : DomainEventBase
{
    public Guid UserId { get; }
    public string Username { get; }
    public string Email { get; }

    public UserRegisteredEvent(Guid userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
}