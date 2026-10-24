using ErpCloud.Api.Data;
using ErpCloud.Api.Models;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ErpDatabase")
    ?? throw new InvalidOperationException("Database connection string is missing.");

builder.Services.AddDbContext<ErpDbContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();

app.MapGet("/health", () => "ERP API is running");
app.MapGet("/api/purchase-requests", async (ErpDbContext db) =>
    await db.PurchaseRequests.AsNoTracking().ToListAsync());
app.MapPost("/api/purchase-requests", async (PurchaseRequest request, ErpDbContext db) =>
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

    if (await db.PurchaseRequests.AnyAsync(r => r.Id == request.Id))
    {
        return Results.Conflict(new
        {
            error = "A purchase request with this ID already exists."
        });
    }

    db.PurchaseRequests.Add(request);
    await db.SaveChangesAsync();
    return Results.Created(
        $"/api/purchase-requests/{request.Id}",
        request
    );
});
app.MapGet("/api/purchase-requests/count", async (ErpDbContext db) =>
    await db.PurchaseRequests.CountAsync());
app.MapGet("/api/purchase-requests/{id:int}", async (int id, ErpDbContext db) =>
{
    var request = await db.PurchaseRequests
    .AsNoTracking()
    .FirstOrDefaultAsync(r => r.Id == id);

    if (request is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(request);
});
app.Run();
