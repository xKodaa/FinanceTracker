using FinanceTracker.Utility;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FinanceTracker.Model.Services
{
    public class CurrencyApiService
    {
        private readonly string API_HOST = "https://api.frankfurter.app";
        private readonly HttpClient client;

        public CurrencyApiService()
        {
            client = new()
            {
                BaseAddress = new Uri(API_HOST)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Konverze měn z API
        public async Task<string> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                string request = $"/latest?amount={amount}&from={fromCurrency}&to={toCurrency}";
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();

                // Přeformatovat string aby se vrátil v přijatelném formátu a uložil do labelu
                string substring = content.Substring(content.IndexOf(toCurrency));
                string value = substring.Replace("}", "").Replace("\"", "").Replace(":", "").Replace(toCurrency, "");
                string result = $"{value} {toCurrency}";

                Logger.WriteLog(this, $"({API_HOST}): Poslána žádost na konverzi měn '{fromCurrency}->{toCurrency}'");
                return result;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(this, $"({API_HOST}): Chyba při dotazu konverze '{fromCurrency}->{toCurrency}': {ex.Message}");
            }
            return "";
        }

        // Získání všech dostupných měn z API
        public async Task<List<Currency>> GetAvailableCurrenciesAsync()
        {
            try
            {
                string request = "/currencies";
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                Logger.WriteLog(this, $"({API_HOST}): Poslána žádost na získání všech dostupných měn");

                Dictionary<string, string>? currencyDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                List<Currency> currencies = [];
                if (currencyDict != null)
                {
                    int index = 1;
                    foreach (var kvp in currencyDict)
                    {
                        currencies.Add(new Currency
                        {
                            Id = index++,
                            Code = kvp.Key,
                            Name = kvp.Value
                        });
                    }
                }
                return currencies;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(this, $"Chyba při získávání dostupných měn při dotazu na {API_HOST}: {ex.Message}");
            }
            return [];
        }
    }
}
