namespace LunarLensBackend.DTOs;

public class RegisterResponse
{
    public Guid Id { get; set; }  
    public string Email { get; set; } 
    public List<string> Errors { get; set; }
}