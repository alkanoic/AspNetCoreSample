using FluentValidation.Results;

namespace AspNetCoreSample.WebApi.Test;

public sealed class CommonValidationTest
{
    [Fact]
    [Trait("Category", nameof(CommonValidationTest))]
    public void GetValidationErrorsReturnsErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name1", "文字列は必須です"),
            new ValidationFailure("Name1", "文字列は100文字以内で入力してください")
        };
        var validationResult = new ValidationResult(failures);

        var errors = CommonValidation.GetValidationErrors(validationResult);

        Assert.NotNull(errors);
        Assert.Equal(2, errors.Count);
        Assert.Equal("Name1", errors[0].Field);
        Assert.Equal("文字列は必須です", errors[0].Message);
        Assert.Equal("Name1", errors[1].Field);
        Assert.Equal("文字列は100文字以内で入力してください", errors[1].Message);
    }

    [Fact]
    [Trait("Category", nameof(CommonValidationTest))]
    public void GetValidationErrorsWithNoErrorsReturnsEmptyList()
    {
        var validationResult = new ValidationResult();

        var errors = CommonValidation.GetValidationErrors(validationResult);

        Assert.NotNull(errors);
        Assert.Empty(errors);
    }
}
