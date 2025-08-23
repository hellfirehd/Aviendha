# Task-Specific Guidance: Adding a New Feature to the Billing Management Module

## **Objective**
Add a feature to allow users to download invoices as PDFs in the Billing Management module.

---

## **Step-by-Step Guidance**

### 1. **Define the Feature Flag**
- Add a feature flag to control the visibility of the new feature.
- Use the ABP Feature Management system:

```csharp
public static class BillingManagementFeatures
{
    public const string InvoiceDownload = "BillingManagement.InvoiceDownload";
}
```

- Register the feature in the `FeatureDefinitionProvider`:

```csharp
context.Add(
    new FeatureDefinition(
        BillingManagementFeatures.InvoiceDownload,
        defaultValue: "false",
        displayName: "Enable Invoice Download",
        description: "Allows users to download invoices as PDFs."
    )
);
```

### 2. **Update the Application Contracts Layer**
- Add a DTO for the invoice download request:

```csharp
public class InvoiceDownloadRequestDto
{
    public Guid InvoiceId { get; set; }
}
```

- Add a service interface:

```csharp
public interface IInvoiceAppService
{
    Task<FileDto> DownloadInvoiceAsync(InvoiceDownloadRequestDto input);
}
```

### 3. **Implement the Application Layer**
- Implement the `IInvoiceAppService` in the `Application` project:

```csharp
public class InvoiceAppService : ApplicationService, IInvoiceAppService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceAppService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<FileDto> DownloadInvoiceAsync(InvoiceDownloadRequestDto input)
    {
        var invoice = await _invoiceRepository.GetAsync(input.InvoiceId);
        if (invoice == null)
        {
            throw new EntityNotFoundException(typeof(Invoice), input.InvoiceId);
        }

        // Generate PDF (use a library like iTextSharp or QuestPDF)
        var pdfBytes = GenerateInvoicePdf(invoice);

        return new FileDto("Invoice.pdf", "application/pdf", pdfBytes);
    }

    private byte[] GenerateInvoicePdf(Invoice invoice)
    {
        // Logic to generate PDF
    }
}
```

### 4. **Update the Presentation Layer**
- Add a Blazor component for the invoice download button:

```razor
@inject IInvoiceAppService InvoiceAppService

<button @onclick="DownloadInvoice">Download Invoice</button>

@code {
    private async Task DownloadInvoice()
    {
        var file = await InvoiceAppService.DownloadInvoiceAsync(new InvoiceDownloadRequestDto
        {
            InvoiceId = Guid.NewGuid() // Replace with actual ID
        });

        // Trigger file download in the browser
        await JSRuntime.InvokeVoidAsync("downloadFile", file.Name, file.ContentType, file.Content);
    }
}
```

### 5. **Add Unit Tests**
- Write unit tests for the `InvoiceAppService`:

```csharp
public class InvoiceAppService_Tests : BillingManagementApplicationTestBase
{
    private readonly IInvoiceAppService _invoiceAppService;

    public InvoiceAppService_Tests()
    {
        _invoiceAppService = GetRequiredService<IInvoiceAppService>();
    }

    [Fact]
    public async Task Should_Throw_Exception_If_Invoice_Not_Found()
    {
        // Arrange
        var input = new InvoiceDownloadRequestDto { InvoiceId = Guid.NewGuid() };

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(() => _invoiceAppService.DownloadInvoiceAsync(input));
    }
}
```

### 6. **Enable the Feature**
- Enable the feature flag in the development environment:

```csharp
context.SetFeatureValue(BillingManagementFeatures.InvoiceDownload, "true");
```

### 7. **Document the Feature**
- Update the module's documentation to include details about the new feature, its usage, and any configuration steps.

---

This task-specific guidance provides a clear, actionable path for implementing a new feature while adhering to the project's architecture, coding standards, and best practices.