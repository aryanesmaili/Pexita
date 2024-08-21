using Pexita.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pexita.Services
{
    public class IranAPI : IIranAPI
    {
        private const string _apiAddress = "https://iran-locations-api.vercel.app/api/v1/";

        public async Task<bool> IsStateValid(string StateName, bool isEng)
        {
            string urlToWorkWith = _apiAddress + (isEng ? "en/" : "fa/") + $"states?state={StateName}";
            using (HttpClient _httpClient = new())
            {
                HttpResponseMessage response = await _httpClient.GetAsync(urlToWorkWith);

                if (response.IsSuccessStatusCode)
                {
                    State? result = (await response.Content.ReadFromJsonAsync<List<State>?>() ?? [])[0];
                    return !string.IsNullOrEmpty(result?.Center);
                }
            };
            return false;
        }
        public async Task<bool> IsCityValid(string StateName, string CityName, bool isEng)
        {
            string urlToWorkWith = _apiAddress + (isEng ? "en/" : "fa/") + $"cities?state={StateName}";
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(urlToWorkWith);
                if (response.IsSuccessStatusCode)
                {
                    using var jsonStream = await response.Content.ReadAsStreamAsync();
                    using var jsonDocument = await JsonDocument.ParseAsync(jsonStream);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    List<City>? citiesArray = jsonDocument.RootElement[0]
                        .GetProperty("cities")
                        .Deserialize<List<City>>(options)
                        ?? throw new ArgumentException($"State {StateName} is not valid");

                    City? isCity = citiesArray.FirstOrDefault(x => x.Name == CityName);
                    return isCity == null
                        ? throw new ArgumentException($"City {CityName} is not in {StateName}.")
                        : true;
                }
                return false;
            }
        }
    }
    public class City
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = string.Empty;

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class State : City
    {
        public required string Center { get; set; }
    }
}


