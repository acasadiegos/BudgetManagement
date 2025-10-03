using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "This field cannot be more than {1} characteres")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; }
        public int UserId { get; set; }
    }
}
