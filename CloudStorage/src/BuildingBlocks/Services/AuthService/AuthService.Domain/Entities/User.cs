using SharedKernel;
using AuthService.Domain.Events;

namespace AuthService.Domain;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    public List<string> Roles { get; private set; } = new List<string>();

    private User() { }

    public User(string username, string email, string passwordHash, string salt)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Salt = salt;
        Roles.Add(UserRoles.User);

        AddDomainEvent(new UserRegisteredEvent(Id, Username, Email));
    }

    public void AddRole(string role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
            MarkAsUpdated();
        }
    }

    public void RemoveRole(string role)
    {
        if (Roles.Contains(role))
        {
            Roles.Remove(role);
            MarkAsUpdated();
        }
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        MarkAsUpdated();
    }

    public void UpdatePassword(string passwordHash, string salt)
    {
        PasswordHash = passwordHash;
        Salt = salt;
        MarkAsUpdated();
    }


}