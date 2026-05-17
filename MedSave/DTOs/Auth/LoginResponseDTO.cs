namespace MedSave.DTOs.Auth;

public class LoginResponseDTO
{
    public string Token { get; set; }
    public long UserId { get; set; }
    public string NameUser { get; set; }
    public string Email { get; set; }
    public long RoleUserId { get; set; }
    public long ProfUserId { get; set; }
}