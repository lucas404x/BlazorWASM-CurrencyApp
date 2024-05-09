using System.Text.Json.Nodes;
using System.Text.Json;

namespace CurrencyApp.Client.Helpers;

public static class FreeCurrencyApiHelper
{
    private static readonly IReadOnlyList<string> _currencyApiCodes = [
        "EUR", "USD", "JPY", "BGN", "CZK", "DKK", "GBP", "HUF", "PLN", "RON",
        "SEK", "CHF", "ISK", "NOK", "HRK", "RUB", "TRY", "AUD", "BRL", "CAD",
        "CNY", "HKD", "IDR", "ILS", "INR", "KRW", "MXN", "MYR", "NZD", "PHP",
        "SGD", "THB", "ZAR"];

    public static List<T> GetActualCurrencyResult<T>(string dataJson)
    {
        List<T> result = [];

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, PropertyNameCaseInsensitive = true };
        var jsonNode = JsonNode.Parse(dataJson, new JsonNodeOptions { PropertyNameCaseInsensitive = true });
        jsonNode = jsonNode?["data"];
        if (jsonNode is null)
        {
            throw new InvalidDataException("Parsing response error.");
        }

        foreach (var code in _currencyApiCodes)
        {
            var node = jsonNode[code];
            if (node is not null)
            {
                var obj = node.Deserialize<T>(jsonOptions);
                if (obj is not null) result.Add(obj);
            }
        }

        return result;
    }

    public static bool IsCurrencyExists(string currencyCode) => _currencyApiCodes.Contains(currencyCode);
}
