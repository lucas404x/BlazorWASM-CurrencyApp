using CurrencyApp.Client.Dtos;
using CurrencyApp.Client.Services;
using Microsoft.AspNetCore.Components;

namespace CurrencyApp.Client.Pages;

public partial class Currency
{
    private class CurrencyInfoModel
    {
        public List<CountryDto> Countries { get; set; } = [];

        public string? SelectedCountryFromCode { get; set; }
        public string? SelectedCountryToCode { get; set; }

        public decimal Amount { get; set; } = 1;
        public decimal ExchangeRate { get; set; }

        public decimal ConvertedAmount => Amount * ExchangeRate;

        public bool IsValid()
        {
            if (SelectedCountryFromCode is null || SelectedCountryToCode is null) return false;
            else if (Amount < 1 || ExchangeRate <= 0) return false;
            return true;
        }

        public void Swap()
            => (SelectedCountryToCode, SelectedCountryFromCode) = (SelectedCountryFromCode, SelectedCountryToCode);
    }

    private bool DisableControls = false;

    [Inject]
    private ICurrencyService CurrencyService { get; set; } = null!;

    private CurrencyInfoModel Model { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        DisableControls = true;
        
        await SetInitialCountries();
        await RefreshExchangeRate();
        
        DisableControls = false;

        await base.OnInitializedAsync();
    }

    private async Task SetInitialCountries()
    {
        Model.Countries = await CurrencyService.GetCountriesAsync();
        Model.SelectedCountryFromCode = Model.Countries[0].Code;
        Model.SelectedCountryToCode = Model.Countries[1].Code;
    }

    private async Task RefreshExchangeRate()
    {
        if (Model.SelectedCountryFromCode is null || Model.SelectedCountryToCode is null) return;
        Model.ExchangeRate = await CurrencyService.GetExchangeRateAsync(Model.SelectedCountryFromCode, Model.SelectedCountryToCode);
    }

    private async Task OnSetNewCountry()
    {
        DisableControls = true;
        await RefreshExchangeRate();
        DisableControls = false;
    }
    
    private async Task SwapCountries()
    {
        DisableControls = true;

        Model.Swap();
        await RefreshExchangeRate();

        DisableControls = false;
    }
}
