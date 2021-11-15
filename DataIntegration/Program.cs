using System.Data;

namespace DataIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var _controller = new SQLController(new SqlConnection("Server=52.157.108.214;Database=Caspar;User Id=Caspar;Password=TestCase;"));
            var apiResult = getCurrentValutaString().Result;
            var updateDate = getUpdateDate(apiResult);

            _controller.ClearTable();
            _controller.InsertMultiple(getConversions(apiResult, updateDate));
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

        public static IEnumerable<ValutaConversion> getConversions(string apiResult, DateTime updateDate)
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
