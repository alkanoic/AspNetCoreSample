using System.Data.Common;

namespace DbContainer.Test;

public sealed class PostgresTest : IAsyncDisposable
{
    private readonly PostgresFixture _fixture;
    private readonly DbConnection _dbConnection;

    public PostgresTest()
    {
        _fixture = new PostgresFixture();
        // initialize synchronously for simplicity
        _fixture.InitializeAsync().GetAwaiter().GetResult();

        _dbConnection = _fixture.DbConnection;
        _dbConnection.Open();
    }

    public ValueTask DisposeAsync()
    {
        _dbConnection.Dispose();
        return _fixture.DisposeAsync();
    }

    [Test]
    public async Task NamesTableContainsName()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT name FROM name;";

        // Whening
        using var dataReader = command.ExecuteReader();

        // Then
        await Assert.That(dataReader.Read()).IsTrue();
        await Assert.That(dataReader.GetString(0)).IsEqualTo("太郎");
        await Assert.That(dataReader.Read()).IsTrue();
        await Assert.That(dataReader.GetString(0)).IsEqualTo("花子");
        await Assert.That(dataReader.Read()).IsTrue();
        await Assert.That(dataReader.GetString(0)).IsEqualTo("令和");
    }
}
