using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Test;

public sealed class MvcInProcessTest : PageTest, IAsyncDisposable
{
    private readonly WebApplicationFactoryFixture<Program> _factory;

    public MvcInProcessTest()
    {
        _factory = new WebApplicationFactoryFixture<Program>();
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    [Test]
    [Category(nameof(MvcInProcessTest))]
    public async Task GetIndexPlaywright()
    {
        _factory.CreateDefaultClient();

        await PlaywrightRetry.RunAsync(async () =>
        {
            await Page.GotoAsync($"{_factory.HostUrl}");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Auth" }).ClickAsync();
            await Page.GetByLabel("ユーザー名またはメールアドレス").FillAsync("admin");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "パスワード" }).FillAsync("admin");
            await Task.WhenAll([Page.GetByRole(AriaRole.Button, new() { Name = "サインイン" }).ClickAsync(), Page.WaitForURLAsync($"{_factory.HostUrl}/Auth")]);

            await Assert.That(await Page.TitleAsync()).Contains("Auth Page");

            await Page.GetByRole(AriaRole.Link, new() { Name = "Sample" }).ClickAsync();
            await Assert.That(await Page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
        });
    }
}
