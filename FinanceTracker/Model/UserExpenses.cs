using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    public class UserExpenses
    {
        public int Price { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        public UserExpenses(int price, string category, DateTime date)
        {
            Price = price;
            Category = category;
            Date = date;
        }

        override public string ToString()
        {
            return $"{Date}: {Category} - {Price},-";
        }
    }
}
