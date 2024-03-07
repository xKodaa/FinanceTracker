﻿using FinanceTracker.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    // Třída pro uložení objektů z konfiguračního souboru projektu
    public class AppConfig
    {
        public string? ConnectionString { get; set; }
        public int? StocksRefreshRate { get; set; }
        public int? CryptoRefreshRate { get; set; }

        public AppConfig() { }

        public bool IsInitialized() 
        {
            return ConnectionString != null && StocksRefreshRate != null && CryptoRefreshRate != null;  
        }
    }
}