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
        public string ConnectionString { get; set; }
        public int CryptoRefreshRate { get; set; }
        public string DefaultCurrency { get; set; }
        public List<string> FinanceCategories { get; set; }

        public AppConfig() 
        {
            ConnectionString = String.Empty;
            DefaultCurrency = String.Empty;
            CryptoRefreshRate = 0;
            FinanceCategories = [];
        }

        public bool IsInitialized() 
        {
            return ConnectionString != null && DefaultCurrency != null && CryptoRefreshRate != 0 && FinanceCategories != null;  
        }

        public override string ToString()
        {
            return $"ConnectionString = {ConnectionString}, DefaultCurrency = {DefaultCurrency}, CryptoRefreshRate = {CryptoRefreshRate}, " +
                $"FinanceCategories = {FinanceCategories}";
        }
    }
}
