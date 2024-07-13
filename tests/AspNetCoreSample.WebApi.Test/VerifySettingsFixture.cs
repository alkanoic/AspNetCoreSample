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

[CollectionDefinition(nameof(VerifySettingsFixtures))]
public class VerifySettingsFixtures : ICollectionFixture<VerifySettingsFixture>
{
}
