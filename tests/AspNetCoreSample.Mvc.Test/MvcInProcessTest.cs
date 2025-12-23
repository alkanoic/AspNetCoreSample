namespace AspNetCoreSample.Mvc.Test;

[ClassDataSource<WebApplicationFactoryFixture<Program>>]
public sealed class MvcInProcessTest(WebApplicationFactoryFixture<Program> factory) : PageTest
{
    [Test]
    [Category(nameof(MvcInProcessTest))]
    public async Task GetIndexPlaywright()
    {
        factory.CreateDefaultClient();

        await Page.GotoAsync($"{factory.HostUrl}");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Auth" }).ClickAsync();
        await Page.GetByLabel("ユーザー名またはメールアドレス").FillAsync("admin");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "パスワード" }).FillAsync("admin");
        await Task.WhenAll([Page.GetByRole(AriaRole.Button, new() { Name = "サインイン" }).ClickAsync(), Page.WaitForURLAsync($"{factory.HostUrl}/Auth")]);

        await Assert.That(await Page.TitleAsync()).Contains("Auth Page");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Sample" }).ClickAsync();
        await Assert.That(await Page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
    }
}
