using AspNetCoreSample.WebApi.Models;

using FluentValidation.Results;

namespace AspNetCoreSample.WebApi.Test;

public sealed class WebApiFailResponseTest
{
    [Fact]
    [Trait("Category", nameof(WebApiFailResponseTest))]
    public void DefaultConstructorHasEmptyErrors()
    {
        var response = new WebApiFailResponse();

        Assert.False(response.Success);
        Assert.Null(response.ErrorMessage);
        Assert.Empty(response.Errors);
    }

    [Fact]
    [Trait("Category", nameof(WebApiFailResponseTest))]
    public void ValidationResultConstructorPopulatesErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new("Field1", "Error1"),
            new("Field2", "Error2")
        };
        var validationResult = new ValidationResult(failures);

        var response = new WebApiFailResponse(validationResult);

        Assert.False(response.Success);
        Assert.Null(response.ErrorMessage);
        Assert.Equal(2, response.Errors.Count);
        Assert.Equal("Field1", response.Errors[0].PropertyName);
        Assert.Equal("Error1", response.Errors[0].ErrorMessage);
        Assert.Equal("Field2", response.Errors[1].PropertyName);
        Assert.Equal("Error2", response.Errors[1].ErrorMessage);
    }

    [Fact]
    [Trait("Category", nameof(WebApiFailResponseTest))]
    public void ExceptionConstructorSetsErrorMessage()
    {
        var ex = new InvalidOperationException("test error");

        var response = new WebApiFailResponse(ex);

        Assert.False(response.Success);
        Assert.Equal("test error", response.ErrorMessage);
        Assert.Empty(response.Errors);
    }
}
