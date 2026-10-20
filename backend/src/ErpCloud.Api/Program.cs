var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var purchaseRequests = new List<PurchaseRequest>();

app.MapGet("/health", () => "ERP API is running");

app.MapGet("/api/purchase-requests", () => purchaseRequests);

app.MapPost("/api/purchase-requests", (PurchaseRequest request) =>
{
    purchaseRequests.Add(request);

    return Results.Created("/api/purchase-requests", request);
});

app.MapGet("/api/purchase-requests/count", () => purchaseRequests.Count);

app.Run();

record PurchaseRequest(int Id, string ProductName, int Quantity);