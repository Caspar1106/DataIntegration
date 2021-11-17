namespace DataIntegration
{
    public class ValutaConversionRepository
    {
        private readonly ValutaContext _context;

        public ValutaConversionRepository(ValutaContext context)
        {
            _context = context;
        }

    }
}