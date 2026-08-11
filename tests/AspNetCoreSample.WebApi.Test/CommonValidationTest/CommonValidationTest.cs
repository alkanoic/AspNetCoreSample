using FluentValidation.Results;

namespace AspNetCoreSample.WebApi.Test;

public sealed class CommonValidationTest
{
    [Test]
    [Category(nameof(CommonValidationTest))]
    public async Task GetValidationErrorsReturnsErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name1", "文字列は必須です"),
            new ValidationFailure("Name1", "文字列は100文字以内で入力してください")
        };
        var validationResult = new ValidationResult(failures);

        var errors = CommonValidation.GetValidationErrors(validationResult);

        await Assert.That(errors).IsNotNull();
        await Assert.That(errors.Count).IsEqualTo(2);
        await Assert.That(errors[0].Field).IsEqualTo("Name1");
        await Assert.That(errors[0].Message).IsEqualTo("文字列は必須です");
        await Assert.That(errors[1].Field).IsEqualTo("Name1");
        await Assert.That(errors[1].Message).IsEqualTo("文字列は100文字以内で入力してください");
    }
    [Test]
    [Category(nameof(CommonValidationTest))]
    public async Task GetValidationErrorsWithNoErrorsReturnsEmptyList()
    {
        var validationResult = new ValidationResult();

        var errors = CommonValidation.GetValidationErrors(validationResult);

        await Assert.That(errors).IsNotNull();
        await Assert.That(errors).IsEmpty();
    }
}
