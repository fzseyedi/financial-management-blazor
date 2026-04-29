using FinancialManagement.Blazor.Components;
using FinancialManagement.Blazor.Models.Common;
using FinancialManagement.Blazor.Services.Auth;
using FinancialManagement.Blazor.Services.Customers;
using FinancialManagement.Blazor.Services.Invoices;
using FinancialManagement.Blazor.Services.Payments;
using FinancialManagement.Blazor.Services.Products;
using FinancialManagement.Blazor.Services.Reports;
using FinancialManagement.Blazor.Services.Users;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ApiSettings>(
        builder.Configuration.GetSection(ApiSettings.SectionName));

// Auth services (scoped per Blazor Server circuit)
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationCore();

// DelegatingHandler no longer used in the pipeline — token is attached
// directly by the circuit-scoped FinancialApiClient to avoid scope mismatch.

// Authenticated API client — used by FinancialApiClient
builder.Services.AddHttpClient("FinancialApi", (sp, client) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
});

// Anonymous client — used only for login (no token required)
builder.Services.AddHttpClient("FinancialApiAnonymous", (sp, client) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
});

builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<FinancialApiClient>();
builder.Services.AddScoped<CustomerApiService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<InvoiceApiService>();
builder.Services.AddScoped<PaymentApiService>();
builder.Services.AddScoped<ReportApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<RoleApiService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous(); // HTTP pipeline defers to Blazor circuit auth (AuthorizeRouteView)

app.Run();
