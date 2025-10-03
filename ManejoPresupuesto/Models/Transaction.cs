using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "You must select a categorie")]

        [Display(Name = "Categorie")]
        public int CategorieId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "The note cannot have more than {1} chars")]
        public string Note { get; set; } = string.Empty;
        [Range(1, maximum: int.MaxValue, ErrorMessage = "You must select an account")]

        [Display(Name = "Account")]
        public int AccountId { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; } = OperationType.Income;
        public string Account { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
    }
}
