namespace Bulky.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    [ValidateNever]
    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Count { get; set; }

    [Required]
    public string UserId { get; set; }

    [ValidateNever]
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    [NotMapped]
    public double Price { get; set; }
}
