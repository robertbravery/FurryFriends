# External Services Integration

## Background Check Service Integration

### Configuration
```csharp
public class BackgroundCheckConfiguration
{
    public string ApiKey { get; set; }
    public string ApiEndpoint { get; set; }
    public int TimeoutSeconds { get; set; }
    public bool RequireSSN { get; set; }
    public string[] RequiredChecks { get; set; }
}
```

### Service Implementation
```csharp
public interface IBackgroundCheckService
{
    Task<BackgroundCheckResult> InitiateCheck(BackgroundCheckRequest request);
    Task<BackgroundCheckStatus> GetStatus(string checkId);
    Task<BackgroundCheckReport> GetReport(string checkId);
}

public class BackgroundCheckService : IBackgroundCheckService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BackgroundCheckService> _logger;
    
    public async Task<BackgroundCheckResult> InitiateCheck(BackgroundCheckRequest request)
    {
        // Implementation
    }
}
```

## Payment Processing Integration

### Stripe Integration
```csharp
public class StripePaymentService : IPaymentService
{
    private readonly IStripeClient _stripeClient;
    
    public async Task<PaymentAccount> CreatePaymentAccount(PetWalker walker)
    {
        var options = new AccountCreateOptions
        {
            Type = "express",
            Country = walker.Address.Country,
            Email = walker.Email.Value,
            BusinessType = "individual",
            Capabilities = new AccountCapabilitiesOptions
            {
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
            }
        };
        
        // Implementation
    }
}
```

## Document Storage Service

### Azure Blob Storage Implementation
```csharp
public class AzureBlobDocumentService : IDocumentService
{
    private readonly BlobServiceClient _blobServiceClient;
    
    public async Task<DocumentMetadata> StoreDocument(
        Stream content, 
        string fileName, 
        string contentType)
    {
        // Implementation
    }
    
    public async Task<Document> GetDocument(string documentId)
    {
        // Implementation
    }
}