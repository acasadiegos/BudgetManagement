namespace ManejoPresupuesto.Models
{
    public class GetByMonthResult
    {
        public int Month { get; set; }
        public DateTime ReferenceDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Income { get; set; }
        public decimal Egress { get; set; }
        public OperationType OperationTypeId { get; set; }

    }
}
