namespace ManejoPresupuesto.Models
{
    public class UpdateTransactionViewModel : CreateTransactionViewModel
    {
        public int PreviousAccountId { get; set; }
        public decimal PreviousAmount { get; set; }
        public string UrlRetorno { get; set; } = string.Empty;
    }
}
