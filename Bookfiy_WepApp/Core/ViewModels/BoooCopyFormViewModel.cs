using Bookfiy_WepApp.Core.Const;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class BoooCopyFormViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        [Display(Name = "Is Availabble For Rental?")]
        public bool IsAvailabbleForRent { get; set; }
        [Display(Name ="Eddition Number"),Range(1,1000,ErrorMessage = ErrorValidation.InvalidRange)]
        public int EdditionNumber { get; set; }
        public bool ShowRentalInput { get; set; }
    }
}
