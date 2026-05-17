using MedSave.DTOs.Auth;

namespace MedSave.Services.UsersSys;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
}