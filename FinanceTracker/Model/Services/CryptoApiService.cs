﻿using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json;

namespace FinanceTracker.Model.Services
{
    public class CryptoApiService
    {
        private static readonly string API_HOST = "https://api.coincap.io";
        private readonly HttpClient client;

        public CryptoApiService() 
        {
            client = new()
            {
                BaseAddress = new Uri(API_HOST)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<CryptoCurrency>> RetrieveCryptoInfoAsync()
        {
            List<CryptoCurrency> cryptoCurrencies = new();
            try
            {
                HttpResponseMessage response = await client.GetAsync("/v2/assets");
                response.EnsureSuccessStatusCode();
                Logger.WriteLog(this, $"Poslána žádost na {API_HOST}: získání informací o kryptoměnách");
                string responseBody = await response.Content.ReadAsStringAsync();
                CryptoData? cryptoData = JsonSerializer.Deserialize<CryptoData>(responseBody);

                if (cryptoData?.Data != null)
                {
                    return cryptoData.Data;
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(this, $"Chyba při dotazu na {API_HOST}: {ex.Message}");
            }
            return [];
        }
    }
}