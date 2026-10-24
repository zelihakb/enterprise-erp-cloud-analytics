using ErpCloud.Api.Models;
using ErpCloud.Api.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ErpDatabase")
    ?? throw new InvalidOperationException("Database connection string is missing.");

builder.Services.AddDbContext<ErpDbContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();

var purchaseRequests = new List<PurchaseRequest>();

app.MapGet("/health", () => "ERP API is running");

app.MapGet("/api/purchase-requests", () => purchaseRequests);
app.MapPost("/api/purchase-requests", (PurchaseRequest request) =>
{
    if (request.Id <= 0)
    {
        return Results.BadRequest(new
        {
            error = "Request ID must be greater than zero."
        });
    }

    if (string.IsNullOrWhiteSpace(request.ProductName))
    {
        return Results.BadRequest(new
        {
            error = "Product name cannot be empty."
        });
    }

    if (request.Quantity <= 0)
    {
        return Results.BadRequest(new
        {
            error = "Quantity must be greater than zero."
        });
    }

    if (purchaseRequests.Any(r => r.Id == request.Id))
    {
        return Results.Conflict(new
        {
            error = "A purchase request with this ID already exists."
        });
    }

    purchaseRequests.Add(request);

    return Results.Created(
        $"/api/purchase-requests/{request.Id}",
        request
    );
});
app.MapGet("/api/purchase-requests/count", () => purchaseRequests.Count);
app.MapGet("/api/purchase-requests/{id:int}", (int id) =>
{
    var request = purchaseRequests.FirstOrDefault(r => r.Id == id);

    if (request is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(request);
});
app.Run();
