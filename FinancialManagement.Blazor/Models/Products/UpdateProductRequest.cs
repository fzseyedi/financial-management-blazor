using System.ComponentModel.DataAnnotations;

namespace FinancialManagement.Blazor.Models.Products;

public sealed record UpdateProductRequest(
    [property: Required(ErrorMessage = "Code is required.")]
    [property: StringLength(20, ErrorMessage = "Code cannot exceed 20 characters.")]
    string Code,

    [property: Required(ErrorMessage = "Name is required.")]
    [property: StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    string Name,

    [property: Required(ErrorMessage = "Unit price is required.")]
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
    decimal UnitPrice);
