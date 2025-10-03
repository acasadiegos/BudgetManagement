namespace ManejoPresupuesto.Models
{
    public class MonthlyReportViewModel
    {
        public IEnumerable<GetByMonthResult> MonthlyTransactions { get; set; } = new List<GetByMonthResult>();
        public decimal Incomes => MonthlyTransactions.Sum(x => x.Income);
        public decimal Egress => MonthlyTransactions.Sum(x => x.Egress);
        public decimal Total => Incomes - Egress;
        public int Year { get; set; }
    }
}
