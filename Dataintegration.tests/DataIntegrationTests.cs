
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Xunit;
namespace DataIntegration.tests
{
    public class DataIntegrationTests
    {
        private readonly string _InputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../input.txt");
        private readonly string _getUrl = "https://valutakurser.azurewebsites.net/ValutaKurs";
        [Fact]
        public void Status_Code_returns_OK_using_string()
        {
            HttpClient httpClient = new HttpClient();
            var http = httpClient.GetAsync(_getUrl);
            var result = http.Result;
            var actual = result.StatusCode.ToString();
            httpClient.Dispose();

            Assert.Equal("OK", actual);

        }
        [Fact]
        public void Status_Code_returns_OK_using_URI()
        {
            HttpClient httpClient = new HttpClient();
            var http = httpClient.GetAsync(new Uri(_getUrl));
            var result = http.Result;
            var actual = result.StatusCode.ToString();
            httpClient.Dispose();
            Assert.Equal("OK", actual);
        }

        [Fact]
        public void Update_Date_returns_correct_date_and_time()
        {
            var input = System.IO.File.ReadAllText(_InputFilePath);
            var actual = DataIntegration.Program.getUpdateDate(input);
            var expected = new DateTime(2021, 11, 10, 18, 8, 26);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetConversions_returns_IEnumerable_of_ValutaConversions()
        {
            var input = System.IO.File.ReadAllText(_InputFilePath);
            var updateDate = new DateTime(2021, 11, 10, 18, 8, 26);

            //testing the first 3 instances
            var expected = new List<ValutaConversion>()
            {
                new ValutaConversion(0, "DKK", "EUR", updateDate, 14.217386571124935),
                new ValutaConversion(1, "DKK", "USD", updateDate, 15.92314867267027),
                new ValutaConversion(2, "DKK", "SEK", updateDate, 134.099726931630624)
            };

            var conversions = DataIntegration.Program.getConversions(input, updateDate);

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
