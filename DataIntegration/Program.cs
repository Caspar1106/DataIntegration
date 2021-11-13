namespace DataIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Create and open connection to database
            var conn = new SqlConnection("Server=52.157.108.214;Database=Caspar;User Id=Caspar;Password=TestCase;");
            conn.Open();

            //Get the input, and format it into conversions
            var input = getCurrentValutaString();
            var conversions = GetConversions(input.Result);

            //insert into SQL database
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

        public static IEnumerable<ValutaConversion> GetConversions(string apiResult)
        {
            var pattern = @"[a-zA-Z]+.{3}([A-Z]{3}).{3}[a-zA-Z]+.{3}([A-Z]{3}).{3}[a-zA-Z]+.{2}(\d+.\d+)";
            var updateDate = getUpdateDate(apiResult);
           
            foreach (Match m in Regex.Matches(apiResult, pattern))
            {
                yield return new ValutaConversion(m.Groups[1].ToString(), m.Groups[2].ToString(), double.Parse(m.Groups[3].ToString()), updateDate); 
            }
        }

        public static string getUpdateDate(string apiResult) 
        {
            var datePattern = @"[a-zA-Z]+.{3}(\d+.\d+.\d+).(\d+.\d+.\d+.\d+)[A-Z]";
            var match = Regex.Match(apiResult, datePattern);
            return match.Groups[2].ToString() + " " + match.Groups[1].ToString();

        }
    }
}
