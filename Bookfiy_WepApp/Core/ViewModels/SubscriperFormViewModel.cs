using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class SubscriperFormViewModel
    {
        public string? Key { get; set; }

        [MaxLength(100), Display(Name = "First Name"),
            RegularExpression(Regx.DenySpecialCharacters, ErrorMessage = ErrorValidation.DenySpecialCharacters)]
        public string FirstName { get; set; } = null!;

        [MaxLength(100), Display(Name = "Last Name"),
            RegularExpression(Regx.DenySpecialCharacters, ErrorMessage = ErrorValidation.DenySpecialCharacters)]
        public string LastName { get; set; } = null!;

        [Display(Name = "Date Of Birth")]
        [AssertThat("DateOfBirth <= Today()", ErrorMessage = ErrorValidation.NotAllowFutureDates)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [MaxLength(14), Display(Name = "National ID"),
            RegularExpression(Regx.NationalId, ErrorMessage = ErrorValidation.InvalidNationalId)]
        [Remote("AllowNationalId", null!, AdditionalFields = "Key", ErrorMessage = ErrorValidation.Dublicated)]
        public string NantionalId { get; set; } = null!;

        [MaxLength(11), Display(Name = "Mobile Number"),
            RegularExpression(Regx.PhoneNumber, ErrorMessage = ErrorValidation.PhoneNumber)]
        [Remote("AllowMobileNumber", null!, AdditionalFields = "Key", ErrorMessage = ErrorValidation.Dublicated)]
        public string MobileNumber { get; set; } = null!;

        public bool HasWhatsApp { get; set; }

        [MaxLength(150), EmailAddress]
        [Remote("AllowEmail", null!, AdditionalFields = "Key", ErrorMessage = ErrorValidation.Dublicated)]
        public string Email { get; set; } = null!;

        [RequiredIf("Key == ''", ErrorMessage = ErrorValidation.EmptyImage)]
        public IFormFile? Image { get; set; }

        [Display(Name = "Area")]
        public int AreaId { get; set; }

        public IEnumerable<SelectListItem>? Areas { get; set; } = new List<SelectListItem>();

        [Display(Name = "Governorate")]
        public int GovernorateId { get; set; }

        public IEnumerable<SelectListItem>? Governorates { get; set; }

        [MaxLength(500)]
        public string Address { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
    }
}
