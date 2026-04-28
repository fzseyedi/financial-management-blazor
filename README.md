# Financial Management — Blazor Front-End

A professional **Blazor Server** front-end application for managing customers, products, invoices, payments, and financial reports. Built with **.NET 10** and designed to consume a RESTful back-end API.

## Features

- **Customer Management** — Create, edit, activate/deactivate, and delete customers with paginated listing and status filtering.
- **Product Management** — Create, edit, activate/deactivate, and delete products with paginated listing and status filtering.
- **Invoice Management** — Create, edit, issue, and delete invoices with advanced filtering by customer, date range, and status. Support for draft/issued/partially-paid/paid/cancelled status tracking. Optimistic concurrency control via version tokens. Lock editing for non-draft invoices.
- **Line Item Management** — Add, edit, and remove invoice line items with product selection via an interactive product picker modal. Real-time calculation of line totals and invoice grand totals.
- **Reports** — View unpaid invoices report.
- **Shared Components** — Reusable pagination, confirmation modal, and product picker modal components.
- **Structured Logging** — Integrated with [Serilog](https://serilog.net/) for structured, file-based logging.

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | [.NET 10](https://dotnet.microsoft.com/) |
| UI | [Blazor Server](https://learn.microsoft.com/aspnet/core/blazor/) (Interactive Server rendering) |
| HTTP Client | `IHttpClientFactory` with named clients |
| Logging | Serilog (file sink, JSON format, rolling daily) |
| Styling | Bootstrap 5 |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- The **back-end Web API** running locally — see the companion repository:  
  👉 **[financial-management-api](https://github.com/fzseyedi/financial-management-api)**

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/fzseyedi/financial-management-blazor.git
cd financial-management-blazor
```

### 2. Start the back-end API

Follow the instructions in the [financial-management-api](https://github.com/fzseyedi/financial-management-api) repository to run the API. By default it listens on `http://localhost:5182`.

### 3. Configure the API base URL

The Blazor app reads the API address from `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5182"
  }
}
```

Update `BaseUrl` if your API runs on a different port.

### 4. Run the Blazor app

```bash
cd FinancialManagement.Blazor
dotnet run
```

The application will start and be available at the URL shown in the console output (typically `https://localhost:5001` or similar).

## Project Structure

```
FinancialManagement.Blazor/
├── Components/
│   ├── Layout/          # MainLayout, NavMenu, ReconnectModal
│   ├── Pages/           # Home, Error, NotFound
│   └── Shared/          # Pagination, ConfirmationModal, ProductPickerModal
├── Models/
│   ├── Common/          # ApiSettings, PaginatedResponse
│   ├── Customers/       # CustomerDto, Create/Update request records
│   ├── Products/        # ProductDto, Create/Update request records
│   ├── Invoices/        # InvoiceDto, InvoiceItemDto, CreateInvoiceRequest, UpdateInvoiceRequest
│   ├── Payments/        # Payment models
│   └── Reports/         # UnpaidInvoiceDto
├── Pages/
│   ├── Customers/       # Index, Create, Edit
│   ├── Products/        # Index, Create, Edit
│   ├── Invoices/        # Index, Create, Edit
│   ├── Payments/        # Payment pages
│   └── Reports/         # UnpaidInvoices
├── Services/
│   ├── Customers/       # CustomerApiService
│   ├── Products/        # ProductApiService
│   ├── Invoices/        # InvoiceApiService
│   ├── Payments/        # PaymentApiService
│   └── Reports/         # ReportApiService
├── appsettings.json
└── Program.cs
```

## Author

Fariborz Seyedi
Senior .NET Developer
Istanbul, Turkey

LinkedIn: [linkedin.com/in/fariborz-seyedi-49582934b](https://linkedin.com/in/fariborz-seyedi-49582934b)
GitHub: [github.com/fzseyedi](https://github.com/fzseyedi)
