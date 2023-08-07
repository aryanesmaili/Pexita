namespace Pexita.Utility
{
    public class IranAPI
    {
        private const string _apiAddress = "https://iran-locations-api.vercel.app/api/v1/";
        private const string _allProvinces = _apiAddress + "states";
        private const string _citiesOfAState = _apiAddress + "cities?state=";
        private const string _certainState = _apiAddress + "states?id=";
        
    }
}


