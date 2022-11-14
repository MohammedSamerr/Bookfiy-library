﻿using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Core.Models
{
    [Index(nameof(Name) , IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsDelete { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdateOn { get; set; }
    }
}
