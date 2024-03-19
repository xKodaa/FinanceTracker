using FinanceTracker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

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

        public async Task<string> ConvertCurrencyAsync(string fromCurrency, string toCurrency, int amount)
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
                    foreach (var kvp in currencyDict)
                    {
                        currencies.Add(new Currency(kvp.Key, kvp.Value));
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
