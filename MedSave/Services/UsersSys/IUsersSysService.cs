using MedSave.DTOs;
using MedSave.DTOs.UsersSys;

namespace MedSave.Services.UsersSys;

public interface IUsersSysService
{
    Task<CreateUserRequest?> GetByIdAsync(long id);
    Task<IEnumerable<UsersSysDTO>> GetAllAsync();
    Task<UsersSysDTO?> AddAsync(CreateUserRequest createUserRequest);
    Task UpdateAsync(long id, UsersSysDTO usersSysDto);
    Task DeleteAsync(long id);
    Task<PagedResult<UsersSysDTO>> SearchAsync(
        string? nameUser,
        string? email,
        long? roleUserId,
        long? profUserId,
        int page,
        int pageSize,
        string sortBy,
        string sortDir
    );
}