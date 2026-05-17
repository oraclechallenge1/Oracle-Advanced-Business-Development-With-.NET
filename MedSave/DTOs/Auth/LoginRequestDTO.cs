namespace MedSave.DTOs.Auth
{
    public class LoginRequestDTO
    {
        public string Email { get; set; } = string.Empty;

        public string PasswordUser { get; set; } = string.Empty;
    }
}