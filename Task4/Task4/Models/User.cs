namespace Task4.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    //public string Company { get; set; }
    public string PasswordHash { get; set; }
    public UserStatus Status { get; set; }
    public DateTime LastSeen { get; set; }
}
