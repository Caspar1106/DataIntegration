using System.Data;

namespace DataIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Get the input, and format it into conversions
            var input = getCurrentValutaString().Result;
            var updateDate = getUpdateDate(input);
            var conversions = GetConversions(input, updateDate);

            //Create and open connection to database. 'using' ensures that the connection closes after
            using (var conn = new SqlConnection("Server=52.157.108.214;Database=Caspar;User Id=Caspar;Password=TestCase;"))
            {
                conn.Open();
                
                //wipe the table
                clearTable(conn);

                //Allow for inserts into database
                turnOnIdentityInsert(conn);

                //Insert values
                insertValues(conn, conversions);

                //Ensure that new values cannot be added beyond this point
                turnOffIdentityInsert(conn);
                
                
                //Simple print statement to see result
                /* var command = new SqlCommand("Select * from ValutaKurser", conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}", reader[0], reader[1], reader[2], reader[3], reader[4]));
                    }
                } */
            }

        }
        private static async Task<string> getCurrentValutaString()
        {
            HttpClient httpClient = new HttpClient();
            var http = httpClient.GetAsync("https://valutakurser.azurewebsites.net/ValutaKurs");
            var result = http.Result;
            if (result.StatusCode.ToString() != "OK")
            {
                Console.WriteLine("Could not connect to API. Exiting with StatusCode: " + result.StatusCode.ToString());
                System.Environment.Exit(0);
            }

            var content = await result.Content.ReadAsStringAsync();
            return content;
        }

        public static IEnumerable<ValutaConversion> GetConversions(string apiResult, DateTime updateDate)
        {
            var pattern = @"[a-zA-Z]+.{3}([A-Z]{3}).{3}[a-zA-Z]+.{3}([A-Z]{3}).{3}[a-zA-Z]+.{2}(\d+.\d+)";
            int currentId = 0;
            foreach (Match m in Regex.Matches(apiResult, pattern))
            {
                var valutaConversion = new ValutaConversion(currentId, m.Groups[1].ToString(), m.Groups[2].ToString(), updateDate, double.Parse(m.Groups[3].ToString()));
                currentId++;
                yield return valutaConversion;
            }
        }

        public static DateTime getUpdateDate(string apiResult)
        {
            var datePattern = @"[a-zA-Z]+.{3}(\d+).(\d+).(\d+).(\d+).(\d+).(\d+).(\d+)[A-Z]";
            var match = Regex.Match(apiResult, datePattern);
            var updateDate = new DateTime(
                Int32.Parse(match.Groups[1].ToString()),
                Int32.Parse(match.Groups[2].ToString()),
                Int32.Parse(match.Groups[3].ToString()),
                Int32.Parse(match.Groups[4].ToString()),
                Int32.Parse(match.Groups[5].ToString()),
                Int32.Parse(match.Groups[6].ToString()));
            return updateDate;

        }

        public static void clearTable(SqlConnection connection) 
        {
            var cmdStr = "DELETE FROM ValutaKurser";
            using var command = new SqlCommand(cmdStr, connection); 
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Could not Delete from ValutaKurser with error message: " + e.Message);
            }
        }
        public static void turnOnIdentityInsert(SqlConnection connection) 
        {
            var cmdStr = "SET IDENTITY_INSERT ValutaKurser ON";
            using var command = new SqlCommand(cmdStr, connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Could not execute query with this message: " + e.Message);
            }
        }

        public static void turnOffIdentityInsert(SqlConnection connection) 
        {
            var cmdStr = "SET IDENTITY_INSERT ValutaKurser OFF";
            using var command = new SqlCommand(cmdStr, connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Could not execute query with this message: " + e.Message);
            }
        }

        public static void insertValues(SqlConnection connection, IEnumerable<ValutaConversion> conversions) 
        {
            var cmdStr = "INSERT INTO ValutaKurser (id, FromCurrency, ToCurrency, UpdatedAt, Rate) VALUES (@val1, @val2, @val3, @val4, @val5)";
            using var command = new SqlCommand(cmdStr, connection);
            foreach (var val in conversions)
            {
                command.Parameters.AddWithValue("@val1", val.Id);
                command.Parameters.AddWithValue("@val2", val.FromCurrency);
                command.Parameters.AddWithValue("@val3", val.ToCurrency);
                command.Parameters.AddWithValue("@val4", val.UpdatedAt);
                command.Parameters.AddWithValue("@val5", val.Rate);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine("Could not execute query with this message: " + e.Message);
                }
                command.Parameters.Clear();
            }
        }

    }
}
