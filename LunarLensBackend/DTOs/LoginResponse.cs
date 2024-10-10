namespace LunarLensBackend.DTOs;

public class LoginResponse
{
    public string AccessToken { get; set; }  
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }

    public LoginResponse(string access, string refresh, DateTime expires)
    {
        AccessToken = access;
        RefreshToken = refresh;
        Expires = expires;
    }
}
