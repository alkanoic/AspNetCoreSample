namespace AspNetCoreSample.Spa.Test;

// WebAPI と SPA の起動を全テストクラスで共有し、起動を 1 回に抑える。
[CollectionDefinition(nameof(SpaTestFixtures))]
public sealed class SpaTestFixtures : ICollectionFixture<SpaTestFixture>
{
}
