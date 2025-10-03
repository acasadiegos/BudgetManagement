using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class AccountCreationViewModel : Account
    {
        public IEnumerable<SelectListItem> AccountTypes { get; set; } = new List<SelectListItem>();
    }
}
