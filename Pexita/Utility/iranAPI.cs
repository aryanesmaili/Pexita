namespace Pexita.Utility
{
    public class IranAPI : IIranAPI
    {
        private const string _apiAddress = "https://iran-locations-api.vercel.app/api/v1/";
        private const string _allProvinces = _apiAddress + "states";
        private const string _citiesOfAState = _apiAddress + "cities?state=";
        private const string _certainState = _apiAddress + "states?id=";
        private string _urlToWorkWith = "";

        public async Task<bool> IsStateValid(string StateName)
        {
            _urlToWorkWith = _allProvinces;
            using (HttpClient _httpClient = new())
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_urlToWorkWith);

                if (response.IsSuccessStatusCode)
                {
                    List<State> result = await response.Content.ReadFromJsonAsync<List<State>>();

                    State state = result != null ? result.FirstOrDefault(x => x.Name == StateName) : null;

                    return state != null;
                }
            };
            return false;
        }
        public async Task<bool> IsCityValid(string StateName, string CityName)
        {
            _urlToWorkWith = _citiesOfAState + StateName;
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(_urlToWorkWith);

                if (response.IsSuccessStatusCode)
                {
                    List<City> result = await response.Content.ReadFromJsonAsync<List<City>>();

                    City isCity = result != null ? result.FirstOrDefault(x => x.Name == CityName) : null;

                    return isCity != null;
                }
                return false;
            }
        }
    }
    public class City
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int Id { get; set; }
    }

    public class State : City
    {
        public string Center { get; set; }
    }
}


