﻿namespace Bulky.Models.ViewModels;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductViewModel
{
    public Product Product { get; set; }

    [ValidateNever]
    public List<SelectListItem> CategoryList { get; set; }
}
