namespace Bulky.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    public int OrderHeaderId { get; set; }

    [ValidateNever]
    [ForeignKey("OrderHeaderId")]
    public OrderHeader OrderHeader { get; set; }

    public int ProductId { get; set; }

    [ValidateNever]
    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    public int Count { get; set; }

    public double Price { get; set; }
}
