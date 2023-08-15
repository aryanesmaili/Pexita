namespace Pexita.Utility
{
    public interface IIranAPI
    {
        Task<bool> IsCityValid(string StateName, string CityName);
        Task<bool> IsStateValid(string StateName);
    }
}