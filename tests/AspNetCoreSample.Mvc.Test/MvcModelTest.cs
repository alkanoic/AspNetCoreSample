using AspNetCoreSample.Mvc.Models;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcModelTest
{
    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task ErrorViewModel_ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        var model = new ErrorViewModel { RequestId = null };

        await Assert.That(model.ShowRequestId).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task ErrorViewModel_ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        var model = new ErrorViewModel { RequestId = "" };

        await Assert.That(model.ShowRequestId).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task ErrorViewModel_ShowRequestId_WhenRequestIdIsSet_ReturnsTrue()
    {
        var model = new ErrorViewModel { RequestId = "abc-123" };

        await Assert.That(model.ShowRequestId).IsTrue();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task ButtonViewModel_SetsControlName()
    {
        var model = new ButtonViewModel("btn-submit");

        await Assert.That(model.ControlName).IsEqualTo("btn-submit");
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_NameIsRequired()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = null, Email = "test@example.com" });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_NameMaxLength()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "123456", Email = "test@example.com" });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_EmailIsRequired()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = null });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_InvalidEmail()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = "not-email" });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_OptionRequiredWhenNameIsAbc()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "abc", Email = "test@example.com", Option = null });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_OptionMaxLengthWhenNameIsAbc()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "abc", Email = "test@example.com", Option = "1234" });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_NoMustBeGreaterThanOrEqualToZero()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = "test@example.com", No = -1 });

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(MvcModelTest))]
    public async Task FluentViewModelValidator_PassesWithValidData()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = "test@example.com", No = 0 });

        await Assert.That(result.IsValid).IsTrue();
    }
}
