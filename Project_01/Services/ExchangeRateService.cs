using System.Diagnostics.Metrics;
using System.Text.Json;
using Project_01.Interfaces;

namespace Project_01.Services;

using System.Text.Json;

public class ExchangeRateService(HttpClient httpClient, IConfiguration configuration) : IExchangeRateService
{
    private readonly string? _apiKey = configuration["ExchangeRateApi:ApiKey"];
    private const string Url = "https://v6.exchangerate-api.com/v6/";

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        var response = await httpClient.GetAsync(Url + $"{_apiKey}/pair/{fromCurrency}/{toCurrency}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var exchangeRateResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(content);

        if (exchangeRateResponse?.result != "success")
        {
            throw new Exception("Failed to get exchange rate");
        }

        return exchangeRateResponse.conversion_rate;
    }

}

public class ExchangeRateResponse
{
    public string result { get; set; }
    public string documentation { get; set; }
    public string terms_of_use { get; set; }
    public int time_last_update_unix { get; set; }
    public string time_last_update_utc { get; set; }
    public int time_next_update_unix { get; set; }
    public string time_next_update_utc { get; set; }
    public string base_code { get; set; }
    public string target_code { get; set; }
    public decimal conversion_rate { get; set; }
}