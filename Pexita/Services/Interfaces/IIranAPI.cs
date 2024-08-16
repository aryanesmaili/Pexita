namespace Pexita.Utility
{
    public interface IIranAPI
    {
        Task<bool> IsCityValid(string StateName, string CityName, bool isEng);
        Task<bool> IsStateValid(string StateName, bool isEng);
    }
}