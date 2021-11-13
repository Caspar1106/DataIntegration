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

                using (SqlCommand comm = new SqlCommand())
                {
                    //wipe the table
                    comm.Connection = conn;
                    comm.CommandText = "DELETE FROM ValutaKurser";
                    try
                    {
                        comm.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine("Could not Delete from ValutaKurser with error message: " + e.Message);
                    }

                    //Allow for inserts into database
                    comm.CommandText = "SET IDENTITY_INSERT ValutaKurser ON";
                    try
                    {
                        comm.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine("Could not execute query with this message: " + e.Message);
                    }

                    //Insert values
                    string cmdString = "INSERT INTO ValutaKurser (id, FromCurrency, ToCurrency, UpdatedAt, Rate) VALUES (@val1, @val2, @val3, @val4, @val5)";
                
                    comm.CommandText = cmdString;

                    foreach (var val in conversions)
                    {
                        comm.Parameters.AddWithValue("@val1", val.Id);
                        comm.Parameters.AddWithValue("@val2", val.FromCurrency);
                        comm.Parameters.AddWithValue("@val3", val.ToCurrency);
                        comm.Parameters.AddWithValue("@val4", val.UpdatedAt);
                        comm.Parameters.AddWithValue("@val5", val.Rate);

                        try
                        {
                            comm.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            Console.WriteLine("Could not execute query with this message: " + e.Message);
                        }
                        comm.Parameters.Clear();
                    }

                    //Ensure that new values cannot be added beyond this point
                    comm.CommandText = "SET IDENTITY_INSERT ValutaKurser OFF";
                    try
                    {
                        comm.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine("Could not execute query with this message: " + e.Message);
                    }
                }
                
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
    }
}
