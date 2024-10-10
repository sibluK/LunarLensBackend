namespace LunarLensBackend.DTOs;

public class RefreshResponse
{
    public string AccessToken { get; set; }
    public DateTime ExpiresIn { get; set; }

    public RefreshResponse(string accessToken, DateTime expiresIn)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
    }
    
}