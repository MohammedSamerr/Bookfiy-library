﻿namespace Bookfiy_WepApp.Core.ViewModels
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public bool IsDelete { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdateOn { get; set; }
    }
}
