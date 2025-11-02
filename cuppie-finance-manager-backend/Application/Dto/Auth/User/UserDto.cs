namespace Application.Dto.Auth.User;

public class UserDto(int id, string username, string email)
{
    public int Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
}

