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

namespace FinanceTracker.Model
{
    public class CurrencyApiService
    {
        private readonly string API_HOST = "https://api.frankfurter.app";
        private readonly HttpClient client = new HttpClient();

        public CurrencyApiService() { }

        public async void ConvertCurrencyAsync(string fromCurrency, string toCurrency)
        {
            try 
            {
                client.BaseAddress = new Uri(API_HOST);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string request = $"/latest?from={fromCurrency}&to={toCurrency}";
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                Logger.WriteLog(this, $"Poslána žádost na konverzi měn '{fromCurrency}->{toCurrency}'");
                Util.ShowInfoMessageBox(content);
            } catch (Exception ex) 
            {
                Logger.WriteErrorLog(this, $"Chyba při dotazu konverze '{fromCurrency}->{toCurrency}' na {API_HOST}: {ex.Message}");
            }
        }
    }
}
