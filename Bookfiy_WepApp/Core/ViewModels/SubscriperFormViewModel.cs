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
        public int Id { get; set; }

        [Display(Name ="First Name"),MaxLength(100),
           RegularExpression(Regx.DenySpecialCharacters,ErrorMessage =ErrorValidation.DenySpecialCharacters) ]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name"), MaxLength(100),
           RegularExpression(Regx.DenySpecialCharacters, ErrorMessage = ErrorValidation.DenySpecialCharacters)]
        public string LastName { get; set; } = null!;

        [Display(Name = "Date Of Birth")]
        [AssertThat("DateOfBirth <= Today()", ErrorMessage = ErrorValidation.date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Display(Name = "Nantional Id"),MaxLength(14),
            RegularExpression(Regx.NationalId, ErrorMessage = ErrorValidation.InvalidNationalId)]
        [Remote("AllowNationalId", null!, AdditionalFields = "Id", ErrorMessage = ErrorValidation.Dublicated)]
        public string NantionalId { get; set; } = null!;

        [MaxLength(11), Display(Name = "Mobile Number"),
            RegularExpression(Regx.PhoneNumber, ErrorMessage = ErrorValidation.PhoneNumber)]
        [Remote("AllowMobileNumber", null!, AdditionalFields = "Id", ErrorMessage = ErrorValidation.Dublicated)]
        public string MobileNumber { get; set; } = null!;

        [MaxLength(150),Display(Name ="E-mail")]
        [Remote("AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = ErrorValidation.Dublicated)]
        public string Email { get; set; } = null!;
        public bool HasWhatsApp { get; set; }

        [RequiredIf("Id == 0", ErrorMessage = ErrorValidation.EmptyImage)]
        public IFormFile? Image { get; set; }

        [Display(Name = "Area")]
        public int AreaId { get; set; }
        public IEnumerable<SelectListItem>? Areas { get; set; } = new List<SelectListItem>();

        [Display(Name = "Governorate")]
        public int GovernorateId { get; set; }
        public IEnumerable<SelectListItem>? Governorates { get; set; }

        [MaxLength(500)]
        public string Address { get; set; } = null!;

        public string? ImageUrl { get; set; } = null!;
        public string? ImageThumbnailUrl { get; set; } = null!;
    }
}
