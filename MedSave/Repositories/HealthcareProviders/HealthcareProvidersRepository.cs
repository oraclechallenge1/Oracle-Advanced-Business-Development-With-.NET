using MedSave.Context;
using MedSave.Model;
using MedSave.Repositories.Healthcare_Providers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedSave.Repositories.Healthcare_Providers;

public class HealthcareProvidersRepository : IHealthcareProvidersRepository
{
    private readonly MedSaveContext _context;

    public HealthcareProvidersRepository(MedSaveContext context)
    {
        _context = context;
    }

    public async Task<HealthcareProviders?> GetByIdAsync(long id)
    {
        var search = await _context.HealthcareProviders.FindAsync(id);

        return search;
    }

    public async Task<IEnumerable<HealthcareProviders>> GetAllAsync()
    {
        var search = await _context.HealthcareProviders.ToListAsync();

        return search;
    }

    public async Task AddAsync(HealthcareProviders healthcareProviders)
    {
        _context.HealthcareProviders.AddAsync(healthcareProviders);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(HealthcareProviders healthcareProviders)
    {
        _context.HealthcareProviders.Update(healthcareProviders);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var search = await _context.HealthcareProviders.FindAsync(id);

        _context.HealthcareProviders.Remove(search);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<HealthcareProviders> Items, int TotalItems)> SearchAsync(string? providerName, string? healthcareProviderName, long? providerTypeId, long? addressIdStock, int page, int pageSize, string sortBy, string sortDir)
    {
        var query = _context.HealthcareProviders.AsQueryable();

        if (!string.IsNullOrEmpty(providerName)) query = query.Where(h => h.ProviderName == providerName);
        
        if (!string.IsNullOrEmpty(healthcareProviderName)) query = query.Where(h => h.HealthcareProviderName == healthcareProviderName);

        if (providerTypeId.HasValue) query = query.Where(h => h.ProviderTypeId == providerTypeId.Value);
        
        if (addressIdStock.HasValue) query = query.Where(h => h.AddressIdStock == addressIdStock.Value);
        
        var totalItems = await query.CountAsync();
        
        bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy?.ToLowerInvariant() switch
        {
            "providerName" => desc ? query.OrderByDescending(h => h.ProviderName) : query.OrderBy(h => h.ProviderName),
            "healthcareProviderName" => desc ? query.OrderByDescending(h => h.HealthcareProviderName) : query.OrderBy(h => h.HealthcareProviderName),
            "providerTypeId" => desc ? query.OrderByDescending(h => h.ProviderTypeId) : query.OrderBy(h => h.ProviderTypeId),
            "addressIdStock" => desc ? query.OrderByDescending(h => h.AddressIdStock) : query.OrderBy(h => h.AddressIdStock),
            _ => desc ? query.OrderByDescending(h => h.HealthcareProviderId) : query.OrderBy(h => h.HealthcareProviderId)
        };
        
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
        
        var skip = (page - 1) * pageSize;

        var data = await query.Skip(skip).Take(pageSize).ToListAsync();

        return (data, totalItems);
    }
}