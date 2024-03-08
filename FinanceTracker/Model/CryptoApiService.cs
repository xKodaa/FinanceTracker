using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    public class CryptoApiService
    {
        private static readonly string API_HOST = "api.coincap.io";
        static readonly HttpClient client = new HttpClient();

        public CryptoApiService() { }

        private async void ReadAllCryptoInfo()
        {
            client.BaseAddress = new Uri(API_HOST);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("/v2/assets");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
