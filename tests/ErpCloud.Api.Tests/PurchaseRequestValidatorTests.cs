using ErpCloud.Api.Validation;

namespace ErpCloud.Api.Tests;

public class PurchaseRequestValidatorTests
{
    [Theory]
    [InlineData(null, 1, "Product name is required.")]
    [InlineData("", 1, "Product name is required.")]
    [InlineData("   ", 1, "Product name is required.")]
    [InlineData("Keyboard", 0, "Quantity must be positive.")]
    [InlineData("Mouse", -5, "Quantity must be positive.")]
    public void Validate_InvalidInput_ReturnsError(
        string? productName,
        int quantity,
        string expectedError)
    {
        var result = PurchaseRequestValidator.Validate(
            productName,
            quantity
        );

        Assert.Equal(expectedError, result);
    }

    [Fact]
    public void Validate_ValidInput_ReturnsNull()
    {
        var result = PurchaseRequestValidator.Validate("Keyboard", 5);

        Assert.Null(result);
    }
}