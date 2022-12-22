using Bookfiy_WepApp.Core.Const;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(20, ErrorMessage = ErrorValidation.MaxLength), Display(Name = " Full Name")
            ,RegularExpression(Regx.CharactersOnly_Eng , ErrorMessage =ErrorValidation.OnlyEnglish)]
        public string Fullname { get; set; } = null!;

        [MaxLength(20, ErrorMessage =ErrorValidation.MaxLength)]
        [Remote("AllowUser",null!, AdditionalFields ="Id",ErrorMessage =ErrorValidation.Dublicated),
            RegularExpression(Regx.UserName, ErrorMessage =ErrorValidation.NotAllowedUserName)]
        public string UserName { get; set; } = null!;

        [MaxLength(200, ErrorMessage = ErrorValidation.MaxLength), EmailAddress]
        [Remote("AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = ErrorValidation.Dublicated)]
        public string Email { get; set; } = null!;


        [DataType(DataType.Password), StringLength(100,
            ErrorMessage = ErrorValidation.MaxLengthPassword, MinimumLength = 8),
            RegularExpression(Regx.Password,
            ErrorMessage = ErrorValidation.InvalidPass),
            RequiredIf("Id==null", ErrorMessage = "Requred Field")]
        public string? Password { get; set; } = null!;

        [DataType(DataType.Password),Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = ErrorValidation.InvalidPass),
            RequiredIf("Id == null", ErrorMessage = "Required Field")]
        public string? ConfirmPassword { get; set; } = null!;

        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}
