namespace DataIntegration
{
    public class ValutaConversion
    {
        private string FromCurrency { get; set; }
        private string ToCurrency { get; set; }
        private double Rate { get; set; }
        private string UpdatedAt { get; set; }

        public ValutaConversion(string fromCurrency, string toCurrency, double rate, string updatedAt)
        {
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            Rate = rate;
            UpdatedAt = updatedAt;
        }
    }
}