using MedSave.Context;
using MedSave.Model;
using MedSave.Repositories.UsersSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedSave.Repositories.UsersSys;

public class ContactUserRepository : IContactUserRepository
{
    private readonly MedSaveContext _context;

    public ContactUserRepository(MedSaveContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContactUser>> GetAllAsync()
    {
        return await _context.ContactUser.ToListAsync();
    }

    public async Task<ContactUser?> GetByIdAsync(long id)
    {
        return await _context.ContactUser.FindAsync(id);
    }

    public async Task AddAsync(ContactUser contactUser)
    {
        _context.ContactUser.Add(contactUser);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ContactUser contactUser)
    {
        _context.ContactUser.Update(contactUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var search = await _context.ContactUser.FindAsync(id);
        
        _context.ContactUser.Remove(search!);
        await _context.SaveChangesAsync();
    }
}