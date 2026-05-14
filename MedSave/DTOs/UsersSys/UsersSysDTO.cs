namespace MedSave.DTOs.UsersSys;

public class UsersSysDTO
{
    public long UserId { get; set; }
    public string NameUser { get; set; }
    public string Email { get; set; }
    public string PasswordUser { get; set; }
    public long RoleUserId { get; set; }
    public long ProfUserId { get; set; }
    public long ContactUserId { get; set; }
}