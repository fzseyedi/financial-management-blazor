namespace FinancialManagement.Blazor.Models.Users;

public sealed class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
}
