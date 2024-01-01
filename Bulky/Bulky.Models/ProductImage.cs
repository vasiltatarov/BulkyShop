namespace Bulky.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ProductImage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ImageUrl { get; set; }

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}
