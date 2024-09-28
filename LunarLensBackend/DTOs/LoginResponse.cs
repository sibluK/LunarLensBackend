namespace LunarLensBackend.DTOs;

public class LoginResponse
{
    public string Jwt { get; set; }  
    public string Email { get; set; } 
    public string[] Roles { get; set; } 
    

    public LoginResponse(string jwt, string email, string[] roles = null)
    {
        Jwt = jwt;
        Email = email;
        Roles = roles ?? Array.Empty<string>(); 
    }
}
