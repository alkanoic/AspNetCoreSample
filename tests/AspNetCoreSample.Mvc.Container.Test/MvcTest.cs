using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace AspNetCoreSample.Mvc.Container.Test;

[ClassDataSource<MvcDbFixture>]
public sealed class MvcTest(MvcDbFixture mvcFixture) : PageTest
{
    [Test]
    [Category(nameof(MvcTest))]
    public async Task GetIndexPlaywright()
    {
        mvcFixture.SetBaseAddress();

        await PlaywrightRetry.RunAsync(async () =>
        {
            await Page.GotoAsync(mvcFixture.BaseAddress!.ToString());

            await Assert.That(await Page.TitleAsync()).Contains("AspNetCoreSample.Mvc");
        });
    }
}
