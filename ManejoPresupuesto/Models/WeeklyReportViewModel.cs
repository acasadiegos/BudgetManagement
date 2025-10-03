namespace ManejoPresupuesto.Models
{
    public class WeeklyReportViewModel
    {
        public decimal Incomes => WeeklyTransactions.Sum(x => x.Incomes);
        public decimal Egress => WeeklyTransactions.Sum(x => x.Egress);
        public decimal Total => Incomes - Egress;
        public DateTime DateReference { get; set; }
        public IEnumerable<GetByWeekResult> WeeklyTransactions { get; set; } = new List<GetByWeekResult>();
    }
}
