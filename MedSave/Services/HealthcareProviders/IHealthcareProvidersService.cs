using MedSave.DTOs;
using MedSave.DTOs.HealthcareProviders;

namespace MedSave.Services.HealthcareProviders;

public interface IHealthcareProvidersService
{
    Task<CreateHealthcareProviderRequest?> GetByIdAsync(long id);
    Task<IEnumerable<HealthcareProvidersDTO>> GetAllAsync();
    Task<HealthcareProvidersDTO?> AddAsync(CreateHealthcareProviderRequest createHealthcareProviderRequest);
    Task UpdateAsync(long id, HealthcareProvidersDTO healthcareProvidersDto);
    Task DeleteAsync(long id);
    Task<PagedResult<HealthcareProvidersDTO>> SearchAsync(
        string? providerName,
        string? healthcareProviderName,
        long? providerTypeId,
        long? addressIdStock,
        int page,
        int pageSize,
        string sortBy,
        string sortDir
    );
}