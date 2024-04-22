namespace FinanceTracker.Model
{
    // Třída pro uložení objektů z konfiguračního souboru projektu
    public class AppConfig
    {
        public string ConnectionString { get; set; }
        public int CryptoRefreshRate { get; set; }

        public AppConfig() 
        {
            ConnectionString = String.Empty;
            CryptoRefreshRate = 0;
        }

        public bool IsInitialized() 
        {
            return ConnectionString != null && CryptoRefreshRate != 0;  
        }

        public override string ToString()
        {
            return $"ConnectionString = {ConnectionString}, CryptoRefreshRate = {CryptoRefreshRate}";
        }
    }
}
