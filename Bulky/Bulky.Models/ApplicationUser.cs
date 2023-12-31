﻿namespace Bulky.Models;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; }

    public string StreetAddress { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public int? CompanyId { get; set; }

    [ValidateNever]
    [ForeignKey("CompanyId")]
    public Company Company { get; set; }

    [NotMapped]
    public string Role { get; set; }

    [NotMapped]
    public string UserCompanyName { get; set; }
}
