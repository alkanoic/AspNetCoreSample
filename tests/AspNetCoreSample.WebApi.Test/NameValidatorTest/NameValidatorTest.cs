using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.WebApi.Validators;

namespace AspNetCoreSample.WebApi.Test;

public sealed class NameValidatorTest
{
    [Test]
    [Category(nameof(NameValidatorTest))]
    public async Task ValidateValidNamePasses()
    {
        var validator = new NameValidator();
        var name = new Name { Id = 0, Name1 = "テスト" };

        var result = validator.Validate(name);

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(NameValidatorTest))]
    public async Task ValidateEmptyNameFails()
    {
        var validator = new NameValidator();
        var name = new Name { Id = 0, Name1 = "" };

        var result = validator.Validate(name);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.ErrorMessage == "文字列は必須です")).IsTrue();
    }
    [Test]
    [Category(nameof(NameValidatorTest))]
    public async Task ValidateTooLongNameFails()
    {
        var validator = new NameValidator();
        var name = new Name { Id = 0, Name1 = new string('a', 101) };

        var result = validator.Validate(name);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors.Any(e => e.ErrorMessage == "文字列は100文字以内で入力してください")).IsTrue();
    }
}
