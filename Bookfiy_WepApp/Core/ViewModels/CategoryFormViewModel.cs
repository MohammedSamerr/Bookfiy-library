using Microsoft.AspNetCore.Mvc;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Remote("AllowItem", null, AdditionalFields ="Id" , ErrorMessage = "This category is already exsist!")]
        [MaxLength(100, ErrorMessage = "Max Length cannot be more than 100 chr.")]
        public string Name { get; set; } = null!;
    }
}
