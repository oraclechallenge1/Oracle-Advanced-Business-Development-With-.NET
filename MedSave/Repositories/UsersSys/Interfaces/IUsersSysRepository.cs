namespace MedSave.Repositories.UsersSys.Interfaces;

public interface IUsersSysRepository
{
    Task<Model.UsersSys?> GetByEmailAsync(string email);
    Task<Model.UsersSys?> GetByIdAsync(long id);
    Task<IEnumerable<Model.UsersSys>> GetAllAsync();
    Task AddAsync(Model.UsersSys usersSys);
    Task UpdateAsync(Model.UsersSys usersSys);
    Task DeleteAsync(long id);
    Task<(IEnumerable<Model.UsersSys> Items, int TotalItems)> SearchAsync(
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