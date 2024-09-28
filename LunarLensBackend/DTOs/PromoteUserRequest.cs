namespace LunarLensBackend.DTOs;

public class PromoteUserRequest
{
    public string UserEmail { get; set; }
    public string NewRole { get; set; }
}