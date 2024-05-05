using System.Data.Common;

namespace AspNetCoreSample.Mvc.Test;

public sealed class DbTest : IClassFixture<MvcDbFixture>, IDisposable
{
    private readonly DbConnection _dbConnection;

    public DbTest(MvcDbFixture db)
    {
        _dbConnection = MvcDbFixture.DbConnection;
        _dbConnection.Open();
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }

    [Fact]
    public void UsersTableContainsJohnDoe()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT name FROM name;";

        // Whening
        using var dataReader = command.ExecuteReader();

        // Then
        Assert.True(dataReader.Read());
        Assert.Equal("太郎", dataReader.GetString(0));
        Assert.True(dataReader.Read());
        Assert.Equal("花子", dataReader.GetString(0));
        Assert.True(dataReader.Read());
        Assert.Equal("令和", dataReader.GetString(0));
    }
}
