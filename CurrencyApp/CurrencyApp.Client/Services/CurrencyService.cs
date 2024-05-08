using CurrencyApp.Client.Dtos;
using CurrencyApp.Client.Helpers;

namespace CurrencyApp.Client.Services;

public interface ICurrencyService
{
    Task<List<CountryDto>> GetCountriesAsync();

    Task<(decimal, decimal)> GetCurrenciesAsync(string firstCode, string secondCode);
}

public class CurrencyService(HttpClient httpClient) : ICurrencyService
{
    public async Task<List<CountryDto>> GetCountriesAsync()
    {
        var response = await httpClient.GetStringAsync("currencies");
        return FreeCurrencyApiHelper.GetActualCurrencyResult<CountryDto>(response);
    }

    public async Task<(decimal, decimal)> GetCurrenciesAsync(string firstCode, string secondCode)
    {
        ValidateCode(firstCode);
        ValidateCode(secondCode);

        string queryParams = Uri.EscapeDataString($"{firstCode},{secondCode}");
        var response = await httpClient.GetStringAsync($"latest?={queryParams}");
        var currencies = FreeCurrencyApiHelper.GetActualCurrencyResult<decimal>(response);
        if (currencies.Count != 2)
        {
            throw new InvalidDataException("Broken response");
        }
        return (currencies[0], currencies[1]);
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || !FreeCurrencyApiHelper.IsCurrencyExists(code))
            throw new ArgumentException($"The code '{code}' is not valid.");
    }
}
