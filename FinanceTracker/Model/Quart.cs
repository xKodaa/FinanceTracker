namespace FinanceTracker.Model
{
    public class Quart
    {
        public int Order { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Quart(int order, DateTime startDate, DateTime endDate)
        {
            Order = order;
            StartDate = startDate;
            EndDate = endDate;
        }

        public override string ToString()
        {
            return $"{Order}. kvartál ({StartDate:dd.MM} - {EndDate:dd.MM})";
        }
    }
}
