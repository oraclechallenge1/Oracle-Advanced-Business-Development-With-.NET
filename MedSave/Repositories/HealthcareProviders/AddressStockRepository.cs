using MedSave.Context;
using MedSave.Model;
using MedSave.Repositories.Healthcare_Providers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedSave.Repositories.Healthcare_Providers;

public class AddressStockRepository : IAddressStockRepository
{
    private readonly MedSaveContext _context;
    
    public AddressStockRepository(MedSaveContext context)
    {
        _context = context;
    }

    public async Task<AddressStock?> GetByIdAsync(long id)
    {
        return await _context.AddressStock.FindAsync(id);
    }
    
    public async Task<IEnumerable<AddressStock>> GetAllAsync()
    {
        return await _context.AddressStock.ToListAsync();
    }

    public async Task AddAsync(AddressStock addressStock)
    {
        _context.AddressStock.AddAsync(addressStock);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AddressStock addressStock)
    {
        _context.AddressStock.Update(addressStock);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var search = await _context.AddressStock.FindAsync(id);

        _context.AddressStock.Remove(search);
        await _context.SaveChangesAsync();
    }
}