namespace DataIntegration
{
    public class SQLController
    {
        private readonly SqlConnection _connection;
        public SQLController(SqlConnection connection)
        {
            _connection = connection;
        }

        public void Insert(ValutaConversion conversion)
        {
            string cmdString = "INSERT INTO ValutaKurser (id, FromCurrency, ToCurrency, UpdatedAt, Rate) VALUES (@val1, @val2, @val3, @val4, @val5)";

            using var command = new SqlCommand(cmdString, _connection);

            command.Parameters.AddWithValue("@val1", conversion.Id);
            command.Parameters.AddWithValue("@val2", conversion.FromCurrency);
            command.Parameters.AddWithValue("@val3", conversion.ToCurrency);
            command.Parameters.AddWithValue("@val4", conversion.UpdatedAt);
            command.Parameters.AddWithValue("@val5", conversion.Rate);

            openConnection();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to execute non query with error message: {0}", e.Message);
            }
            finally
            {
                closeConnection();
            }
        }

        public void InsertMultiple(IEnumerable<ValutaConversion> conversions)
        {
            turnOnIdentityInserts();
            foreach (var conv in conversions)
            {
                Insert(conv);
            }
            turnOffIdentityInserts();
        }

        public void ClearTable()
        {
            string cmdString = "DELETE FROM ValutaKurser";
            using var command = new SqlCommand(cmdString, _connection);
            openConnection();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to execute non query with error message: {0}", e.Message);
            }
            finally
            {
                closeConnection();
            }
        }
        private void turnOnIdentityInserts()
        {
            string cmdString = "SET IDENTITY_INSERT ValutaKurser ON";
            using (var command = new SqlCommand(cmdString, _connection))
            {
                openConnection();
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Identity Insert is now turned on");
                }
                catch (SqlException e)
                {
                    Console.WriteLine("Failed to execute non query with error message: {0}", e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }

        }
        private void turnOffIdentityInserts()
        {
            string cmdString = "SET IDENTITY_INSERT ValutaKurser OFF";
            using var command = new SqlCommand(cmdString, _connection);

            openConnection();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to execute non query with error message: {0}", e.Message);
            }
            finally
            {
                closeConnection();
            }
        }


        private void openConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private void closeConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}