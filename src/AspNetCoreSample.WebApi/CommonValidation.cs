using FluentValidation.Results;

using NJsonSchema.Validation;

namespace AspNetCoreSample.WebApi;

public static class CommonValidation
{
    public static List<ValidationError> GetValidationErrors(ValidationResult validationResult)
    {
        var errors = new List<ValidationError>();
        foreach (var error in validationResult.Errors)
        {
            errors.Add(new ValidationError
            {
                Field = error.PropertyName,
                Message = error.ErrorMessage
            });
        }
        return errors;
    }
}

public class ValidationError
{
    public required string Field { get; set; }
    public required string Message { get; set; }
}
