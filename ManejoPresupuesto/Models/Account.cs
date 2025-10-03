using ManejoPresupuesto.Validations;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 50)]
        [FirstMayus]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Description { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
    }
}
