namespace FinancialManagement.Blazor.Models.Users;

public sealed class ChangePasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}
