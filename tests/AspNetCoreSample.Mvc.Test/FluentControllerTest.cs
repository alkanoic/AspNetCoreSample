using AspNetCoreSample.Mvc.Controllers;
using AspNetCoreSample.Mvc.Models;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Test;

public sealed class FluentControllerTest
{
    private static FluentController CreateController()
    {
        return new FluentController(new FluentViewModelValidator());
    }

    [Test]
    [Category(nameof(FluentControllerTest))]
    public async Task Index_Get_ReturnsViewWithModel()
    {
        var controller = CreateController();

        var result = controller.Index();

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(viewResult.Model).IsTypeOf<FluentViewModel>();
    }

    [Test]
    [Category(nameof(FluentControllerTest))]
    public async Task Index_Post_WithInvalidModel_ReturnsViewWithErrors()
    {
        var controller = CreateController();
        var vm = new FluentViewModel { Name = null, Email = null };

        var result = await controller.Index(vm);

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(viewResult.Model).IsSameReferenceAs(vm);
        await Assert.That(controller.ModelState.IsValid).IsFalse();
    }

    [Test]
    [Category(nameof(FluentControllerTest))]
    public async Task Index_Post_WithValidModel_AddsModelOnlyErrors()
    {
        var controller = CreateController();
        var vm = new FluentViewModel { Name = "test", Email = "test@example.com", No = 0 };

        var result = await controller.Index(vm);

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(viewResult.Model).IsSameReferenceAs(vm);
        await Assert.That(controller.ModelState.ContainsKey("Name")).IsTrue();
        await Assert.That(controller.ModelState.ContainsKey("")).IsTrue();
    }
}
