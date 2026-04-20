using System.ComponentModel.DataAnnotations;

namespace FinancialManagement.Blazor.Models.Customers;

public sealed record CreateCustomerRequest(
    [property: Required(ErrorMessage = "Code is required.")]
    [property: StringLength(20, ErrorMessage = "Code cannot exceed 20 characters.")]
    string Code,

    [property: Required(ErrorMessage = "Name is required.")]
    [property: StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    string Name,

    [property: StringLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    [property: EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    string? Email,

    [property: StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters.")]
    string? Phone,

    [property: StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    string? Address);
