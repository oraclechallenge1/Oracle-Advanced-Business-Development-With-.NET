using MedSave.Context;
using MedSave.Repositories.UsersSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedSave.Repositories.UsersSys;

public class UsersSysRepository : IUsersSysRepository
{
    private readonly MedSaveContext _context;

    public UsersSysRepository(MedSaveContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Model.UsersSys>> GetAllAsync()
    {
        return await _context.UsersSys.ToListAsync();
    }
    
    public async Task<Model.UsersSys?> GetByIdAsync(long id)
    {
        return await _context.UsersSys.FindAsync(id);
    }

    public async Task AddAsync(Model.UsersSys usersSys)
    {
        _context.UsersSys.Add(usersSys);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Model.UsersSys usersSys)
    {
        _context.UsersSys.Update(usersSys);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var search = await _context.UsersSys.FindAsync(id);
        _context.UsersSys.Remove(search);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Model.UsersSys> Items, int TotalItems)> SearchAsync(string? nameUser, string? email, long? roleUserId, long? profUserId, int page, int pageSize, string sortBy, string sortDir)
    {
        var query = _context.UsersSys.AsQueryable();

        if (!string.IsNullOrEmpty(nameUser)) query = query.Where(u => u.NameUser == nameUser);

        if (!string.IsNullOrEmpty(email)) query = query.Where(u => u.Email == email);

        if (roleUserId.HasValue) query = query.Where(u => u.RoleUserId == roleUserId);

        if (profUserId.HasValue) query = query.Where(u => u.ProfUserId == profUserId);

        var totalItems = await query.CountAsync();

        bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy?.ToLowerInvariant() switch
        {
            "nameUser" => desc ? query.OrderByDescending(u => u.NameUser) : query.OrderBy(u => u.NameUser),
            "email" => desc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "roleUserId" => desc ? query.OrderByDescending(u => u.RoleUserId) : query.OrderBy(u => u.RoleUserId),
            "profUserId" => desc ? query.OrderByDescending(u => u.ProfUserId) : query.OrderBy(u => u.ProfUserId),
            _ => desc ? query.OrderByDescending(u => u.UserId) : query.OrderBy(u => u.UserId)
        };
        
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
        
        var skip = (page - 1) * pageSize;

        var data = await query.Skip(skip).Take(pageSize).ToListAsync();

        return (data, totalItems);
    }
}