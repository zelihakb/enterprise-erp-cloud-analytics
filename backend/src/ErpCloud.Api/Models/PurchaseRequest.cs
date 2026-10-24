namespace ErpCloud.Api.Models;

public record PurchaseRequest(
    int Id,
    string ProductName,
    int Quantity
);