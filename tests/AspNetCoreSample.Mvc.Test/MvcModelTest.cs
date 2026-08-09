using AspNetCoreSample.Mvc.Models;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcModelTest
{
    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void ErrorViewModel_ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        var model = new ErrorViewModel { RequestId = null };

        Assert.False(model.ShowRequestId);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void ErrorViewModel_ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        var model = new ErrorViewModel { RequestId = "" };

        Assert.False(model.ShowRequestId);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void ErrorViewModel_ShowRequestId_WhenRequestIdIsSet_ReturnsTrue()
    {
        var model = new ErrorViewModel { RequestId = "abc-123" };

        Assert.True(model.ShowRequestId);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void ButtonViewModel_SetsControlName()
    {
        var model = new ButtonViewModel("btn-submit");

        Assert.Equal("btn-submit", model.ControlName);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_NameIsRequired()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = null, Email = "test@example.com" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_NameMaxLength()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "123456", Email = "test@example.com" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_EmailIsRequired()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = null });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_InvalidEmail()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = "not-email" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_OptionRequiredWhenNameIsAbc()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "abc", Email = "test@example.com", Option = null });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_OptionMaxLengthWhenNameIsAbc()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "abc", Email = "test@example.com", Option = "1234" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(MvcModelTest))]
    public void FluentViewModelValidator_PassesWithValidData()
    {
        var validator = new FluentViewModelValidator();

        var result = validator.Validate(new FluentViewModel { Name = "test", Email = "test@example.com", No = -1 });

        Assert.True(result.IsValid);
    }
}
