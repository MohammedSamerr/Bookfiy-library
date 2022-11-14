﻿using Bookfiy_WepApp.Core.Const;
using Microsoft.AspNetCore.Mvc;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }


        [MaxLength(100, ErrorMessage = Error.MaxLength), Display(Name = "Author")]
        [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = Error.Dublicated)]
        public string Name { get; set; } = null!;
    }
}
