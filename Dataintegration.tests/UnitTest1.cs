
using Xunit;
namespace DataIntegration.tests
{
    public class DataIntegrationTests
    {
        private string InputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../input.txt");
        private string getUrl = "https://valutakurser.azurewebsites.net/ValutaKurs";
        [Fact]
        public void Status_Code_returns_OK_using_string()
        {
            HttpClient httpClient = new HttpClient();
            var http = httpClient.GetAsync(getUrl);
            var result = http.Result;
            var actual = result.StatusCode.ToString();
            httpClient.Dispose();

            Assert.Equal("OK", actual);

        }
        [Fact]
        public void Status_Code_returns_OK_using_URI()
        {
            HttpClient httpClient = new HttpClient();
            var http = httpClient.GetAsync(new Uri(getUrl));
            var result = http.Result;
            var actual = result.StatusCode.ToString();
            httpClient.Dispose();
            Assert.Equal("OK", actual);
        }

        [Fact]
        public void Update_Date_returns_correct_date_and_time()
        {
            var input = System.IO.File.ReadAllText(InputFilePath);
            var actual = DataIntegration.Program.getUpdateDate(input);
            var expected = "18:08:26.43467 2021-11-10";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetConversions_returns_IEnumerable_of_ValutaConversions()
        {
            var input = System.IO.File.ReadAllText(InputFilePath);
            var updateDate = "18:08:26.43467 2021-11-10";

            //testing the first 3 instances
            var expected = new List<ValutaConversion>()
            {
                new ValutaConversion("DKK", "EUR", 14.217386571124935, updateDate),
                new ValutaConversion("DKK", "USD", 15.92314867267027, updateDate),
                new ValutaConversion("DKK", "SEK", 134.099726931630624, updateDate)
            };

            var conversions = DataIntegration.Program.GetConversions(input);

            var actual = new List<ValutaConversion>();
            using (var iterator = conversions.GetEnumerator())
            {
                for (int i = 0; i < 3; i++)
                {
                    iterator.MoveNext();
                    actual.Add(iterator.Current);
                }
            }
            //convert to JSON string objects to easily compare two collections
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

    }

}
