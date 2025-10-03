namespace ManejoPresupuesto.Models
{
    public class GetByWeekResult
    {
        public int Week { get; set; }
        public decimal Amount { get; set; }
        public OperationType OperationTypeId { get; set; }
        public decimal Incomes { get; set; }
        public decimal Egress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
