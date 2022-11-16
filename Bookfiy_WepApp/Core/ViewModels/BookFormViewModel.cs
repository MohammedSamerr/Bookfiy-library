using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = Error.MaxLength)]
        [Remote("AllowItem", null, AdditionalFields = "Id,AuthorId", ErrorMessage = Error.Dublicated)]
        public string Title { get; set; } = null!;

        [Display(Name ="Author")]
        [Remote("AllowItem", null, AdditionalFields = "Id,Title", ErrorMessage = Error.Dublicated)]
        public int AuthorId { get; set; }
        //dropdown list of Authors
        public IEnumerable<SelectListItem>? Author { get; set; }

        [MaxLength(200, ErrorMessage = Error.MaxLength)]
        public string Publisher { get; set; } = null!;

        [Display(Name = "Publishing Date")]
        [AssertThat("PublishingDate <= Today()" ,ErrorMessage = Error.date)]
        public DateTime PublishingDate { get; set; } = DateTime.Now;

        public IFormFile? Image { get; set; }
        public string? ImageURL { get; set; }

        [MaxLength(50, ErrorMessage = Error.MaxLength)]
        public string Hall { get; set; } = null!;
        [Display(Name = "Is availabble for rent")]
        public bool IsAvailabbleForRent { get; set; }

        public string Description { get; set; } = null!;

        [Display(Name ="Categories")]
        public IList<int> SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
