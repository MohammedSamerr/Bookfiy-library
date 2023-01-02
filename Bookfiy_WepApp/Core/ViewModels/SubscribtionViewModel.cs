using Bookfiy_WepApp.Core.Const;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class SubscribtionViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status 
        { 
            get
            {
                return DateTime.Today > EndDate ? SubscribtionStatus.Expired : DateTime.Today < StartDate ? string.Empty : SubscribtionStatus.Active;
            }
        }
    }
}
