using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    public class CryptoCurrency
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("supply")]
        public string Supply { get; set; }

        [JsonPropertyName("maxSupply")]
        public string MaxSupply { get; set; }

        [JsonPropertyName("marketCapUsd")]
        public string MarketCapUsd { get; set; }

        [JsonPropertyName("volumeUsd24Hr")]
        public string VolumeUsd24Hr { get; set; }

        [JsonPropertyName("priceUsd")]
        public string PriceUsd { get; set; }

        [JsonPropertyName("changePercent24Hr")]
        public string ChangePercent24Hr { get; set; }

        [JsonPropertyName("vwap24Hr")]
        public string Vwap24Hr { get; set; }

        [JsonPropertyName("explorer")]
        public string Explorer { get; set; }

        public CryptoCurrency(string id, string rank, string symbol, string name, string supply, string maxSupply,
                      string marketCapUsd, string volumeUsd24Hr, string priceUsd, string changePercent24Hr,
                      string vwap24Hr, string explorer)
        {
            Id = id;
            Rank = rank;
            Symbol = symbol;
            Name = name;
            Supply = supply;
            MaxSupply = maxSupply;
            MarketCapUsd = marketCapUsd;
            VolumeUsd24Hr = volumeUsd24Hr;
            PriceUsd = priceUsd;
            ChangePercent24Hr = changePercent24Hr;
            Vwap24Hr = vwap24Hr;
            Explorer = explorer;
        }
    }

    public class CryptoData
    {
        [JsonPropertyName("data")]
        public List<CryptoCurrency> Data { get; set; }

        public CryptoData(List<CryptoCurrency> data)
        {
            Data = data;
        }
    }
}
