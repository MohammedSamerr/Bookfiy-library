namespace Bookfiy_WepApp.Core.Models
{
    public class BaseModel
    {
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdateOn { get; set; }

        public string? LastUpdateById { get; set; }
        public ApplicationUser? LastUpdateBy { get; set; }
    }
}
