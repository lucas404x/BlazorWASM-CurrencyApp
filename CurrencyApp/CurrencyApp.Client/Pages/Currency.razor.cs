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
    }

    private bool _IsExchangeRateLoading = false;

    [Inject]
    private ICurrencyService CurrencyService { get; set; } = null!;

    private CurrencyInfoModel Model { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        await SetInitialCountries();
        await RefreshExchangeRate();
        await base.OnInitializedAsync();
    }

    protected override bool ShouldRender() => !_IsExchangeRateLoading;

    private async Task SetInitialCountries()
    {
        Model.Countries = await CurrencyService.GetCountriesAsync();
        Model.SelectedCountryFromCode = Model.Countries[0].Code;
        Model.SelectedCountryToCode = Model.Countries[1].Code;
    }

    private async Task RefreshExchangeRate()
    {
        if (Model.SelectedCountryFromCode is null || Model.SelectedCountryToCode is null) return;
        _IsExchangeRateLoading = true;
        Model.ExchangeRate = await CurrencyService.GetExchangeRateAsync(Model.SelectedCountryFromCode, Model.SelectedCountryToCode);
        _IsExchangeRateLoading = false;
        StateHasChanged();
    }
}
