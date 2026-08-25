using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.Mvc.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.Mvc.Test;

public sealed class NameControllerTest
{
    private static SampleContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SampleContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new SampleContext(options);
    }

    private static NameController CreateController(SampleContext context)
    {
        return new NameController(context);
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Index_ReturnsViewWithNames()
    {
        using var context = CreateContext();
        context.Names.AddRange(
            new Name { Id = 1, Name1 = "太郎" },
            new Name { Id = 2, Name1 = "花子" });
        await context.SaveChangesAsync(CancellationToken.None);
        var controller = CreateController(context);

        var result = await controller.Index();

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        var model = await Assert.That(viewResult.Model).IsTypeOf<List<Name>>();
        await Assert.That(model).Count().IsEqualTo(2);
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Details_WithNullId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Details(null);

        await Assert.That(result).IsTypeOf<NotFoundResult>();
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Details_WithUnknownId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Details(999);

        await Assert.That(result).IsTypeOf<NotFoundResult>();
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Details_WithValidId_ReturnsView()
    {
        using var context = CreateContext();
        context.Names.Add(new Name { Id = 1, Name1 = "太郎" });
        await context.SaveChangesAsync(CancellationToken.None);
        var controller = CreateController(context);

        var result = await controller.Details(1);

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        var model = await Assert.That(viewResult.Model).IsTypeOf<Name>();
        await Assert.That(model.Name1).IsEqualTo("太郎");
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Create_Get_ReturnsView()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = controller.Create();

        await Assert.That(result).IsTypeOf<ViewResult>();
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Create_Post_WithValidModel_RedirectsToIndex()
    {
        using var context = CreateContext();
        var controller = CreateController(context);
        var name = new Name { Id = 0, Name1 = "新規" };

        var result = await controller.Create(name);

        var redirect = await Assert.That(result).IsTypeOf<RedirectToActionResult>();
        await Assert.That(redirect.ActionName).IsEqualTo(nameof(NameController.Index));
        await Assert.That(await context.Names.CountAsync(CancellationToken.None)).IsEqualTo(1);
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Create_Post_WithInvalidModel_ReturnsView()
    {
        using var context = CreateContext();
        var controller = CreateController(context);
        controller.ModelState.AddModelError("Name1", "必須です");
        var name = new Name { Id = 0, Name1 = "" };

        var result = await controller.Create(name);

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        await Assert.That(viewResult.Model).IsSameReferenceAs(name);
        await Assert.That(await context.Names.CountAsync(CancellationToken.None)).IsEqualTo(0);
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Edit_Get_WithNullId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Edit(null);

        await Assert.That(result).IsTypeOf<NotFoundResult>();
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Edit_Get_WithValidId_ReturnsView()
    {
        using var context = CreateContext();
        context.Names.Add(new Name { Id = 1, Name1 = "太郎" });
        await context.SaveChangesAsync(CancellationToken.None);
        var controller = CreateController(context);

        var result = await controller.Edit(1);

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        var model = await Assert.That(viewResult.Model).IsTypeOf<Name>();
        await Assert.That(model.Name1).IsEqualTo("太郎");
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Edit_Post_WithMismatchedId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);
        var name = new Name { Id = 2, Name1 = "更新" };

        var result = await controller.Edit(1, name);

        await Assert.That(result).IsTypeOf<NotFoundResult>();
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Edit_Post_WithValidModel_RedirectsToIndex()
    {
        using var context = CreateContext();
        context.Names.Add(new Name { Id = 1, Name1 = "太郎" });
        await context.SaveChangesAsync(CancellationToken.None);
        context.ChangeTracker.Clear();
        var controller = CreateController(context);
        var name = new Name { Id = 1, Name1 = "更新後" };

        var result = await controller.Edit(1, name);

        var redirect = await Assert.That(result).IsTypeOf<RedirectToActionResult>();
        await Assert.That(redirect.ActionName).IsEqualTo(nameof(NameController.Index));
        var updated = await context.Names.FindAsync([1], CancellationToken.None);
        await Assert.That(updated!.Name1).IsEqualTo("更新後");
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Delete_Get_WithNullId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Delete(null);

        await Assert.That(result).IsTypeOf<NotFoundResult>();
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task Delete_Get_WithValidId_ReturnsView()
    {
        using var context = CreateContext();
        context.Names.Add(new Name { Id = 1, Name1 = "太郎" });
        await context.SaveChangesAsync(CancellationToken.None);
        var controller = CreateController(context);

        var result = await controller.Delete(1);

        var viewResult = await Assert.That(result).IsTypeOf<ViewResult>();
        var model = await Assert.That(viewResult.Model).IsTypeOf<Name>();
        await Assert.That(model.Name1).IsEqualTo("太郎");
    }

    [Test]
    [Category(nameof(NameControllerTest))]
    public async Task DeleteConfirmed_RemovesNameAndRedirects()
    {
        using var context = CreateContext();
        context.Names.Add(new Name { Id = 1, Name1 = "太郎" });
        await context.SaveChangesAsync(CancellationToken.None);
        var controller = CreateController(context);

        var result = await controller.DeleteConfirmed(1);

        var redirect = await Assert.That(result).IsTypeOf<RedirectToActionResult>();
        await Assert.That(redirect.ActionName).IsEqualTo(nameof(NameController.Index));
        await Assert.That(await context.Names.CountAsync(CancellationToken.None)).IsEqualTo(0);
    }
}
