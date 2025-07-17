namespace FinanceTracker.Model
{
    public class Currency
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }

        public override string ToString()
        {
            return $"{Code} ({Name})";
        }
    }
}
