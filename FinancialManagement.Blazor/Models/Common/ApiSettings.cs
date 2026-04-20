namespace FinancialManagement.Blazor.Models.Common;

public sealed class ApiSettings
{
    public const string SectionName = "ApiSettings";
    public string BaseUrl { get; set; } = string.Empty;
}
