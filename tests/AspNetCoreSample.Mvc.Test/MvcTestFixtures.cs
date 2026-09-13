namespace AspNetCoreSample.Mvc.Test;

// Postgres + Keycloak のコンテナを全テストクラスで共有し、起動を 1 回に抑える。
// リソースの少ない環境でコンテナが並行起動されてフリーズするのを防ぐ。
[CollectionDefinition(nameof(MvcTestFixtures))]
public sealed class MvcTestFixtures : ICollectionFixture<WebApplicationFactoryFixture<Program>>
{
}
