using AspNetCoreSample.WebApi.Models;

using FluentValidation.Results;

namespace AspNetCoreSample.WebApi.Test;

public sealed class WebApiFailResponseTest
{
    [Test]
    [Category(nameof(WebApiFailResponseTest))]
    public async Task DefaultConstructorHasEmptyErrors()
    {
        var response = new WebApiFailResponse();

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.ErrorMessage).IsNull();
        await Assert.That(response.Errors).IsEmpty();
    }
    [Test]
    [Category(nameof(WebApiFailResponseTest))]
    public async Task ValidationResultConstructorPopulatesErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new("Field1", "Error1"),
            new("Field2", "Error2")
        };
        var validationResult = new ValidationResult(failures);

        var response = new WebApiFailResponse(validationResult);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.ErrorMessage).IsNull();
        await Assert.That(response.Errors.Count).IsEqualTo(2);
        await Assert.That(response.Errors[0].PropertyName).IsEqualTo("Field1");
        await Assert.That(response.Errors[0].ErrorMessage).IsEqualTo("Error1");
        await Assert.That(response.Errors[1].PropertyName).IsEqualTo("Field2");
        await Assert.That(response.Errors[1].ErrorMessage).IsEqualTo("Error2");
    }
    [Test]
    [Category(nameof(WebApiFailResponseTest))]
    public async Task ExceptionConstructorSetsErrorMessage()
    {
        var ex = new InvalidOperationException("test error");

        var response = new WebApiFailResponse(ex);

        await Assert.That(response.Success).IsFalse();
        await Assert.That(response.ErrorMessage).IsEqualTo("test error");
        await Assert.That(response.Errors).IsEmpty();
    }
}
