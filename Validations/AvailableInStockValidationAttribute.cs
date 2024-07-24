using OtlobLap.Models;
using System.ComponentModel.DataAnnotations;

namespace OtlobLap.CustomValidations
{
    public class AvailableInStockValidationAttribute(string probertyName) : ValidationAttribute
    {
        private readonly string _ProbertyName = probertyName;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var productInfo = validationContext.ObjectType.GetProperty(_ProbertyName);
            var product = productInfo?.GetValue(validationContext.ObjectInstance, null) as Product;
            if (product != null && value is int quantity && quantity > 0 && quantity <= product.AvailableInStock)
            {
                return ValidationResult.Success;
            }
            return base.IsValid(value, validationContext);
        }
    }
}
