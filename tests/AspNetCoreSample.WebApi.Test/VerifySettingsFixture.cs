namespace AspNetCoreSample.WebApi.Test;

public class VerifySettingsFixture
{
    public VerifySettings VerifySettings { get; }

    public VerifySettingsFixture()
    {
        VerifySettings = new VerifySettings();
        VerifySettings.UseDirectory("snapshots");
    }
}