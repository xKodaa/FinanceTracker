using FinanceTracker.Utility;

namespace FinanceTracker.Model
{
    public class UserCryptoCurrency
    {
        public string Symbol { get; set; }
        public decimal Amount { get; set; }
        public decimal PricePerAmount { get; set; }
        public decimal PricePerAmountRounded
        {
            get
            {
                return decimal.Round(PricePerAmount, 2);
            }
            set { }
        }
        public decimal PricePerKs { get; set; }
        public decimal PricePerKsRounded
        {
            get
            {
                return decimal.Round(PricePerKs, 2);
            }
            set { }
        }
        public DateTime DateOfBuy { get; set; }
        public decimal Difference { get; set; }
        public decimal DifferenceRounded
        {
            get
            {
                return decimal.Round(Difference, 2);
            }
            set { }
        }
        public decimal ActualCryptoPrice { get; set; }
        public decimal ActualCryptoPriceRounded
        {
            get
            {
                return decimal.Round(ActualCryptoPrice, 2);
            }
            set { }
        }


        public UserCryptoCurrency(string symbol, decimal amount, decimal price, DateTime dateOfBuy)
        {
            Symbol = symbol;
            Amount = amount;
            PricePerAmount = price;
            DateOfBuy = dateOfBuy;
            PricePerKs = price / amount;
        }

        public bool FindCryptoFromList(List<CryptoCurrency> cryptoCurrencies)
        {
            CryptoCurrency? crypto = cryptoCurrencies.Find(crypto => crypto.Symbol == this.Symbol);
            if (crypto != null)
            {
                CalculateDifference(crypto);
                return true;
            }
            else 
            {
                Util.ShowErrorMessageBox("Uživatelská kryptoměna nebyla nalezena v načtených kryptoměnách");
                Logger.WriteErrorLog(nameof(UserCryptoCurrency), $"Uživatelská kryptoměna nebyla nalezena v načtených kryptoměnách: {this}");
            }
            return false;
        }

        private void CalculateDifference(CryptoCurrency crypto)
        {
            decimal cryptoBuyPrice = this.PricePerKs;

            if (cryptoBuyPrice > 0)
            {
                decimal cryptoActualPrice = crypto.PriceUsdDecimal;
                decimal percentualDifference = ((cryptoActualPrice - cryptoBuyPrice) / cryptoBuyPrice) * 100;
                this.Difference = percentualDifference;
                this.ActualCryptoPrice = cryptoActualPrice;
            }
            else
            {
                this.Difference = 0;
            }
        }

        public override string ToString()
        {
            return $"Symbol = {Symbol}, amount = {Amount} ks, price per amount = {PricePerAmount}, date of buy = {DateOfBuy}, price per ks = {PricePerKs}";
        }
    }
}
