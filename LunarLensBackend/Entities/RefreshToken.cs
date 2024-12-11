namespace LunarLensBackend.Entities;

public class RefreshToken
{
    public int Id { get; set; }  
    public string Token { get; set; }  
    public string UserId { get; set; }  
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedDate { get; set; }  
    public DateTime? RevokedDate { get; set; }


    public RefreshToken(int id, string token, string userId, DateTime expiryDate, DateTime createdDate)
    {
        Id = id;
        Token = token;
        UserId = userId;
        ExpiryDate = expiryDate;
        CreatedDate = createdDate;
    }
    
    public RefreshToken()
    {
        
    }
    
}