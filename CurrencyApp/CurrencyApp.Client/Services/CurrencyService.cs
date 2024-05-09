using CurrencyApp.Client.Dtos;
using CurrencyApp.Client.Helpers;

namespace CurrencyApp.Client.Services;

public interface ICurrencyService
{
    Task<List<CountryDto>> GetCountriesAsync();
    Task<decimal> GetExchangeRateAsync(string baseCountryCode, string relativeCountryCode);
}

public class CurrencyService(HttpClient httpClient) : ICurrencyService
{
    public async Task<List<CountryDto>> GetCountriesAsync()
    {
        var response = await httpClient.GetStringAsync("currencies");
        return FreeCurrencyApiHelper.GetActualCurrencyResult<CountryDto>(response);
    }

    public async Task<decimal> GetExchangeRateAsync(string baseCountryCode, string currencyCountryCode)
    {
        ValidateCode(baseCountryCode);
        ValidateCode(currencyCountryCode);

        // string endpoint = Uri.EscapeDataString($"{firstCode},{secondCode}");
        // https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_qzSsQpiEvaTfWW5askpzBgRlebc3OxQGgZn6E7a4&currencies=USD&base_currency=BRL
        string response = await httpClient.GetStringAsync($"latest?base_currency={baseCountryCode}&currencies={currencyCountryCode}");
        List<decimal> currencyResponse = FreeCurrencyApiHelper.GetActualCurrencyResult<decimal>(response);
        
        decimal exchangeRate = currencyResponse.FirstOrDefault();
        if (exchangeRate == default)
        {
            throw new InvalidDataException($"The exchange from {baseCountryCode} to {currencyCountryCode} was not found.");
        }
        return Math.Round(exchangeRate, 2);
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || !FreeCurrencyApiHelper.IsCurrencyExists(code))
            throw new ArgumentException($"The code '{code}' is not valid.");
    }
}
