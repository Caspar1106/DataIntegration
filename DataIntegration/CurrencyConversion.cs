namespace DataIntegration
{
    public record CurrencyConversion(int Id, string FromCurrency, string ToCurrency, DateTime UpdatedAt, double Rate);
    
}