using System.Data.SqlClient;
using DataIntegration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dataintegration.tests
{
    public class DatabaseTests
    {
        private readonly ValutaContext _context;
        public DatabaseTests() 
        {
            var connection = new SqlConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ValutaContext>();
            builder.UseSqlite(connection);
            var context = new ValutaContext(builder.Options);
            context.Database.EnsureCreated();
           
            _context = context;


        }
    }
}