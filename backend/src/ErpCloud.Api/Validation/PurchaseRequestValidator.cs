namespace ErpCloud.Api.Validation;

public static class PurchaseRequestValidator
{
    public static string? Validate(string? productName, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            return "Product name is required.";

        if (quantity <= 0)
            return "Quantity must be positive.";

        return null;
    }
}