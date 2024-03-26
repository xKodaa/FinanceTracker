using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    public class UserCryptoCurrency
    {
        public string Symbol { get; set; }
        public decimal Amount { get; set; }
        public decimal PricePerAmount { get; set; }
        public DateTime DateOfBuy { get; set; }
        public decimal Difference { get; set; }

        public UserCryptoCurrency(string symbol, decimal amount, decimal price, DateTime dateOfBuy)
        {
            Symbol = symbol;
            Amount = amount;
            PricePerAmount = price;
            DateOfBuy = dateOfBuy;
        }
    }
}
