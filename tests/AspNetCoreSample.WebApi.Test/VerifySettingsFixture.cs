namespace AspNetCoreSample.WebApi.Test;

public class VerifySettingsFixture
{
    public VerifySettings VerifySettings { get; }

    public VerifySettingsFixture()
    {
        VerifySettings = new VerifyTests.VerifySettings();
        VerifySettings.UseDirectory("snapshots");
    }
}
