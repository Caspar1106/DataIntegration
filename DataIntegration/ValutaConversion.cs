namespace DataIntegration
{
    public class ValutaConversion
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double Rate { get; set; }

        public ValutaConversion(int id, string fromCurrency, string toCurrency, DateTime updatedAt, double rate)
        {
            Id = id;
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            UpdatedAt = updatedAt;
            Rate = rate;
        }
    }
}