namespace MarkItDoneApi.V1.User.Entity;

public class UserEntity
{
    public Guid Id { get; private set; }
    public string Username { get; set; } 
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime? CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public UserEntity(Guid id, string username, string email, string password, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Username = username;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}