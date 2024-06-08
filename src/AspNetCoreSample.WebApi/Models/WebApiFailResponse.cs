using FluentValidation.Results;

namespace AspNetCoreSample.WebApi.Models;

public class WebApiFailResponse
{
    public bool Success { get; set; }
    public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    public WebApiFailResponse() { }

    public WebApiFailResponse(ValidationResult validationResult)
    {
        foreach (var a in validationResult.Errors)
        {
            Errors.Add(new ErrorModel()
            {
                PropertyName = a.PropertyName,
                ErrorMessage = a.ErrorMessage
            });
        }
    }
}

public class ErrorModel
{
    public required string PropertyName { get; set; }
    public required string ErrorMessage { get; set; }
}
