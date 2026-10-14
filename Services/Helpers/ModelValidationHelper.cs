using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public static class ModelValidationHelper
{
    /// <summary>
    /// Validates the specified model using data annotations.
    /// </summary>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <param name="model">The model instance to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the model is invalid.</exception>
    public static void Validate<T>(T model) where T : notnull
    {
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
        if (!isValid)
        {
            var errorMessage = validationResults
                .FirstOrDefault()?.ErrorMessage ??
                $"{nameof(T)} is invalid.";
            throw new ArgumentException(errorMessage);
        }
    }
}
