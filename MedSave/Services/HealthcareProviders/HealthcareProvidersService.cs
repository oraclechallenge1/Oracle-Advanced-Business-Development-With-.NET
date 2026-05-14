using MedSave.Context;
using MedSave.DTOs;
using MedSave.DTOs.HealthcareProviders;
using MedSave.Model;
using MedSave.Repositories.Healthcare_Providers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedSave.Services.HealthcareProviders;

public class HealthcareProvidersService : IHealthcareProvidersService
{
    private readonly MedSaveContext _context;
    private readonly IHealthcareProvidersRepository _healthcareProvidersRepository;
    private readonly IAddressStockRepository _addressStockRepository;
    private readonly IProviderTypeRepository _providerTypeRepository;
    
    public HealthcareProvidersService(MedSaveContext context, IHealthcareProvidersRepository healthcareProvidersRepository, IAddressStockRepository addressStockRepository, IProviderTypeRepository providerTypeRepository)
    {
        _context = context;
        _healthcareProvidersRepository = healthcareProvidersRepository;
        _addressStockRepository = addressStockRepository;
        _providerTypeRepository = providerTypeRepository;
    }
    
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) {}
    }
    
    public async Task<CreateHealthcareProviderRequest?> GetByIdAsync(long id)
    {
        var hcp = await _healthcareProvidersRepository.GetByIdAsync(id);

        if (hcp == null)
        {
            throw new NotFoundException($"Healthcare Providers with Id {id} not found");
        }

        var addStock = await _addressStockRepository.GetByIdAsync(hcp.AddressIdStock);

        if (addStock == null)
        {
            throw new NotFoundException($"Address Stock with Id {id} not found");
        }

        var hcpDTO = new HealthcareProvidersDTO
        {
            AddressIdStock = hcp.AddressIdStock,
            HealthcareProviderId = hcp.HealthcareProviderId,
            HealthcareProviderName = hcp.HealthcareProviderName,
            ProviderName = hcp.ProviderName,
            ProviderTypeId = hcp.ProviderTypeId
        };

        var addStockDTO = new AddressStockDTO
        {
            AddressDescription = addStock.AddressDescription,
            AddressIdStock = addStock.AddressIdStock,
            Cep = addStock.Cep,
            Complement = addStock.Complement,
            NeighId = addStock.NeighId,
            NumberStock = addStock.NumberStock
        };

        return new CreateHealthcareProviderRequest
        {
            AddressStockDto = addStockDTO,
            HealthcareProvidersDto = hcpDTO,
        };
    }

    public async Task<IEnumerable<HealthcareProvidersDTO>> GetAllAsync()
    {
        var hcproviders = await _healthcareProvidersRepository.GetAllAsync();

        return hcproviders.Select(hcprovider => new HealthcareProvidersDTO
        {
            AddressIdStock = hcprovider.AddressIdStock,
            HealthcareProviderId = hcprovider.HealthcareProviderId,
            HealthcareProviderName = hcprovider.HealthcareProviderName,
            ProviderName = hcprovider.ProviderName,
            ProviderTypeId = hcprovider.ProviderTypeId
        }).ToList();
    }

    public async Task<HealthcareProvidersDTO?> AddAsync(CreateHealthcareProviderRequest createHealthcareProviderRequest)
    {
        if (createHealthcareProviderRequest.HealthcareProvidersDto == null) throw new ArgumentNullException(nameof(createHealthcareProviderRequest), "healthcareProvidersDto can't be null");

        if (createHealthcareProviderRequest.HealthcareProvidersDto.ProviderName == null) throw new ArgumentNullException(nameof(createHealthcareProviderRequest.HealthcareProvidersDto), "ProviderName can't be null");
        
        if (createHealthcareProviderRequest.HealthcareProvidersDto.HealthcareProviderName == null) throw new ArgumentNullException(nameof(createHealthcareProviderRequest.HealthcareProvidersDto), "HealthcareProviderName can't be null");
        
        if (createHealthcareProviderRequest.HealthcareProvidersDto.ProviderTypeId == 0) throw new ArgumentNullException(nameof(createHealthcareProviderRequest.HealthcareProvidersDto), "ProviderTypeId can't be null");
        
        if (createHealthcareProviderRequest.AddressStockDto == null) throw new ArgumentNullException(nameof(createHealthcareProviderRequest.AddressStockDto));

        var address = new AddressStock
        {
            AddressDescription = createHealthcareProviderRequest.AddressStockDto.AddressDescription,
            Cep = createHealthcareProviderRequest.AddressStockDto.Cep,
            Complement = createHealthcareProviderRequest.AddressStockDto.Complement,
            NeighId = createHealthcareProviderRequest.AddressStockDto.NeighId,
            NumberStock = createHealthcareProviderRequest.AddressStockDto.NumberStock
        };

        await _addressStockRepository.AddAsync(address);

        var healthcareProvider = new Model.HealthcareProviders
        {
            AddressIdStock = address.AddressIdStock,
            ProviderTypeId = createHealthcareProviderRequest.HealthcareProvidersDto.ProviderTypeId,
            HealthcareProviderName = createHealthcareProviderRequest.HealthcareProvidersDto.HealthcareProviderName,
            ProviderName = createHealthcareProviderRequest.HealthcareProvidersDto.ProviderName,
        };

        await _healthcareProvidersRepository.AddAsync(healthcareProvider);

        return new HealthcareProvidersDTO
        {
            HealthcareProviderId = healthcareProvider.HealthcareProviderId,
            AddressIdStock = healthcareProvider.AddressIdStock,
            HealthcareProviderName = healthcareProvider.HealthcareProviderName,
            ProviderName = healthcareProvider.ProviderName,
            ProviderTypeId = healthcareProvider.ProviderTypeId
        };
    }

    public async Task UpdateAsync(long id, HealthcareProvidersDTO healthcareProvidersDto)
    {
        if (healthcareProvidersDto == null) throw new ArgumentNullException(nameof(healthcareProvidersDto), "healthcareProvidersDto can't be null");

        if (healthcareProvidersDto.ProviderName == null) throw new ArgumentNullException(nameof(healthcareProvidersDto), "ProviderName can't be null");
        
        if (healthcareProvidersDto.HealthcareProviderName == null) throw new ArgumentNullException(nameof(healthcareProvidersDto), "HealthcareProviderName can't be null");
        
        if (healthcareProvidersDto.ProviderTypeId == 0) throw new ArgumentNullException(nameof(healthcareProvidersDto), "ProviderTypeId can't be null");

        var existingHCProvider = await _healthcareProvidersRepository.GetByIdAsync(id);

        if (existingHCProvider == null) throw new NotFoundException($"Healthcare Provider with Id {id} not found");

        healthcareProvidersDto.HealthcareProviderId = id;
        
        existingHCProvider.HealthcareProviderName = healthcareProvidersDto.HealthcareProviderName;
        existingHCProvider.ProviderName = healthcareProvidersDto.ProviderName;
        existingHCProvider.ProviderTypeId = healthcareProvidersDto.ProviderTypeId;

        await _healthcareProvidersRepository.UpdateAsync(existingHCProvider);
    }

    public async Task DeleteAsync(long id)
    {
        var existingHcProvider = await _healthcareProvidersRepository.GetByIdAsync(id);
        
        if (existingHcProvider == null) throw new NotFoundException($"Healthcare Provider with id {id} not found");

        var existingAddressStock = await _addressStockRepository.GetByIdAsync(existingHcProvider.AddressIdStock);

        await _healthcareProvidersRepository.DeleteAsync(id);
        
        if (existingAddressStock != null) await _addressStockRepository.DeleteAsync(existingAddressStock.AddressIdStock);
    }

    public async Task<PagedResult<HealthcareProvidersDTO>> SearchAsync(string? providerName, string? healthcareProviderName, long? providerTypeId, long? addressIdStock,
        int page, int pageSize, string sortBy, string sortDir)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var (items, total) = await _healthcareProvidersRepository.SearchAsync(providerName, healthcareProviderName,
            providerTypeId, addressIdStock, page, pageSize, sortBy ?? "healthcareProviderId", sortDir ?? "asc");

        var dtoItems = items.Select(healthcareProvider => new HealthcareProvidersDTO
        {
            AddressIdStock = healthcareProvider.AddressIdStock,
            HealthcareProviderId = healthcareProvider.HealthcareProviderId,
            HealthcareProviderName = healthcareProvider.HealthcareProviderName,
            ProviderName = healthcareProvider.ProviderName,
            ProviderTypeId = healthcareProvider.ProviderTypeId
        }).ToList();
        
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new PagedResult<HealthcareProvidersDTO>
        {
            Items = dtoItems,
            PageInfo = new PageInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages
            }
        };
    }
}