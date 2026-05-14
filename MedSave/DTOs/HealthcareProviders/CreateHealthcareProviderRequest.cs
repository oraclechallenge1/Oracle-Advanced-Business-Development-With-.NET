namespace MedSave.DTOs.HealthcareProviders;

public class CreateHealthcareProviderRequest
{
    public HealthcareProvidersDTO HealthcareProvidersDto { get; set; } = default!;
    public AddressStockDTO AddressStockDto { get; set; } = default!;
}