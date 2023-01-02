using Bookfiy_WepApp.Core.Models;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class SubscriperViewModel
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string NantionalId { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool HasWhatsApp { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string ImageThumbnailUrl { get; set; } = null!;
        public string? Area { get; set; }
        public string? Governorate { get; set; }
        public string Address { get; set; } = null!;
        public bool IsBlackedListed { get; set; }
        public DateTime CreatedOn { get; set; }

        public IEnumerable<SubscribtionViewModel> Subscriptions { get; set; } = new List<SubscribtionViewModel>() ;

    }
}
