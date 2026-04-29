namespace FinancialManagement.Blazor.Models.Auth;

public sealed class UserInfo
{
    public string Username { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
}
