namespace FinanceTracker.Model
{
    public class UserExpenses
    {
        public decimal Price { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        public UserExpenses(decimal price, string category, DateTime date)
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
