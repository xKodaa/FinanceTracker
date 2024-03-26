﻿using System;
using System.Collections.Generic;
using System.Globalization;
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

        public decimal RankDecimal
        {
            get
            {
                if (decimal.TryParse(Rank, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        public decimal SymbolDecimal
        {
            get
            {
                if (decimal.TryParse(Symbol, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0; 
            }
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("supply")]
        public string Supply { get; set; }

        public decimal SupplyDecimal
        {
            get
            {
                if (decimal.TryParse(Supply, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

        [JsonPropertyName("maxSupply")]
        public string MaxSupply { get; set; }
        
        public decimal MaxSupplyDecimal
        {
            get
            {
                if (decimal.TryParse(MaxSupply, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

        [JsonPropertyName("marketCapUsd")]
        public string MarketCapUsd { get; set; }

        public decimal MarketCapUsdDecimal
        {
            get
            {
                if (decimal.TryParse(MarketCapUsd, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

        [JsonPropertyName("volumeUsd24Hr")]
        public string VolumeUsd24Hr { get; set; }

        public decimal VolumeUsd24HrDecimal
        {
            get
            {
                if (decimal.TryParse(VolumeUsd24Hr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

        [JsonPropertyName("priceUsd")]
        public string PriceUsd { get; set; }

        public decimal PriceUsdDecimal
        {
            get
            {
                if (decimal.TryParse(PriceUsd, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 4);
                }
                return 0;
            }
        }

        [JsonPropertyName("changePercent24Hr")]
        public string ChangePercent24Hr { get; set; }

        public decimal ChangePercent24HrDecimal
        {
            get
            {
                if (decimal.TryParse(ChangePercent24Hr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

        [JsonPropertyName("vwap24Hr")]
        public string Vwap24Hr { get; set; }

        public decimal Vwap24HrDecimal
        {
            get
            {
                if (decimal.TryParse(Vwap24Hr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return decimal.Round(result, 2);
                }
                return 0;
            }
        }

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