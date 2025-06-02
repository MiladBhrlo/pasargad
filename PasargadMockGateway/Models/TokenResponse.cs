namespace PasargadMockGateway.Models;

public class TokenResponse
{
    public string ResultMsg { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<Role> Roles { get; set; } = new();
}

public class Role
{
    public string Authority { get; set; } = string.Empty;
} 