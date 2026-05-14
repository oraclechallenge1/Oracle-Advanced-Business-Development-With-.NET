using MedSave.Model;

namespace MedSave.Repositories.UsersSys.Interfaces;

public interface IContactUserRepository
{
    Task<IEnumerable<ContactUser>> GetAllAsync();
    Task<ContactUser?> GetByIdAsync(long id);
    Task AddAsync(ContactUser contactUser);
    Task UpdateAsync(ContactUser contactUser);
    Task DeleteAsync(long id);
}