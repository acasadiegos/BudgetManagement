namespace ManejoPresupuesto.Models
{
    public class GetTransactionsByUserParams
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set;}

    }
}
