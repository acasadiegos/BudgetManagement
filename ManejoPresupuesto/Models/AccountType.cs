using ManejoPresupuesto.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class AccountType //: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [FirstMayus]
        [Remote(action: "VerifyExistsAccountType", controller: "AccountType", 
            AdditionalFields = nameof(Id))]
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int Order { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Name != null && Name.Length > 0)
        //    {
        //        var firstLetter = Name[0].ToString();

        //        if(firstLetter != firstLetter.ToUpper())
        //        {
        //            yield return new ValidationResult("The first letter must be a capital letter", 
        //                new[] {nameof(Name)});
        //        }
        //    }
        //}
    }
}
