namespace DataIntegration
{
    public class ValutaConversionRepo
    {
        private readonly IValutaContext _context;

        public ValutaConversionRepo(IValutaContext context) => _context = context;
        
    }
}