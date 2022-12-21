using Bookfiy_WepApp.Core.Const;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(20, ErrorMessage = ErrorValidation.MaxLength), Display(Name = " Full Name")]
        public string Fullname { get; set; } = null!;

        [MaxLength(20, ErrorMessage =ErrorValidation.MaxLength)]
        public string UserName { get; set; } = null!;

        [MaxLength(200, ErrorMessage = ErrorValidation.MaxLength), EmailAddress]
        public string Email { get; set; } = null!;


        [DataType(DataType.Password), StringLength(100, ErrorMessage = ErrorValidation.MaxLengthPassword, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password),Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = ErrorValidation.InvalidPass)]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}
