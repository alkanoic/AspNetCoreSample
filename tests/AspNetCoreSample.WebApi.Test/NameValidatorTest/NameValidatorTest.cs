using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.WebApi.Validators;

namespace AspNetCoreSample.WebApi.Test;

public sealed class NameValidatorTest
{
    [Fact]
    [Trait("Category", nameof(NameValidatorTest))]
    public void ValidateValidNamePasses()
    {
        var validator = new NameValidator();
        var name = new Name { Id = 0, Name1 = "テスト" };

        var result = validator.Validate(name);

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(NameValidatorTest))]
    public void ValidateEmptyNameFails()
    {
        var validator = new NameValidator();
        var name = new Name { Id = 0, Name1 = "" };

        var result = validator.Validate(name);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "文字列は必須です");
    }

    [Fact]
    [Trait("Category", nameof(NameValidatorTest))]
    public void ValidateTooLongNameFails()
    {
        var validator = new NameValidator();
        var name = new Name { Id = 0, Name1 = new string('a', 101) };

        var result = validator.Validate(name);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "文字列は100文字以内で入力してください");
    }
}
