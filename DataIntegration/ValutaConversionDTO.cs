namespace DataIntegration
{
    public record ValutaConversionDTO(int Id, string? FromCurrency, string? ToCurrency, DateTime? UpdatedAt, double? Rate);
    
}