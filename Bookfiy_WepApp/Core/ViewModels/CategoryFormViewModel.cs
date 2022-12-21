using Bookfiy_WepApp.Core.Const;
using Microsoft.AspNetCore.Mvc;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = ErrorValidation.MaxLength), Display(Name = "Category")]
        [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = ErrorValidation.Dublicated)]
        public string Name { get; set; } = null!;
    }
}
