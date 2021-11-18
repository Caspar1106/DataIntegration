namespace DataIntegration
{
    public class SqlValutaConversionRepository
    {
        private readonly SqlConnection _connection;

        public SqlValutaConversionRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public void Create(ValutaConversionDTO conversion)
        {
            var cmdText = @"INSERT ValutaKurser (FromCurrency, ToCurrency, UpdatedAt, Rate)
                            VALUES (@FromCurrency, @ToCurrency, @UpdatedAt, @Rate);
                            SELECT SCOPE_IDENTITY()";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@FromCurrency", conversion.FromCurrency);
            command.Parameters.AddWithValue("@ToCurrency", conversion.ToCurrency);
            command.Parameters.AddWithValue("@UpdatedAt", conversion.UpdatedAt);
            command.Parameters.AddWithValue("@Rate", conversion.Rate);

            try
            {
                OpenConnection();
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Could not create new conversion with error message: {0}", e.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        public void ReadAll()
        {
            var command = new SqlCommand("Select * from ValutaKurser", _connection);
            using (var reader = command.ExecuteReader())
            {
            OpenConnection();
                while (reader.Read())
                {
                    Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}", reader[0], reader[1], reader[2], reader[3], reader[4]));
                }
            }
            CloseConnection();
        }

        public void ClearTable()
        {
            var cmdStr = "DELETE FROM ValutaKurser";
            using var command = new SqlCommand(cmdStr, _connection);

            try
            {
                OpenConnection();
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Could not clear table with error message: {0}", e.Message);
            }
            finally
            {
                CloseConnection();
            }

        }

        public void ReSeedIndex()
        {
            var cmdStr = "DBCC CHECKIDENT ('ValutaKurser', RESEED, 0)";
            using var command = new SqlCommand(cmdStr, _connection);

            try
            {
                OpenConnection();
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Could not reset index with error message: {0}", e.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        public DateTime getLastUpdateDate()
        {
            var cmdStr = "SELECT UpdatedAt FROM ValutaKurser ORDER BY id DESC LIMIT 1";
            using var command = new SqlCommand(cmdStr, _connection);

            OpenConnection();

            using var reader = command.ExecuteReader();
            var lastUpdateDate = reader.Read() ? reader.GetDateTime("UpdatedAt") : new DateTime();

            CloseConnection();

            return lastUpdateDate;
        }

        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}