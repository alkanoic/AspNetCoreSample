using System.Data.Common;

namespace DbContainer.Test;

public sealed class PostgresTest : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly DbConnection _dbConnection;

    public PostgresTest(PostgresFixture db)
    {
        _dbConnection = db.DbConnection;
        _dbConnection.Open();
    }

    public void Dispose()
    {
        _dbConnection.Dispose();
    }

    [Fact]
    public void NamesTableContainsName()
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

    [Fact]
    public void EnumSampleTableContainsEnumColumns()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT enum_column FROM enum_sample ORDER BY id;";

        // When
        using var dataReader = command.ExecuteReader();

        // Then
        Assert.True(dataReader.Read());
        Assert.Equal(0, dataReader.GetInt32(0));
        Assert.True(dataReader.Read());
        Assert.Equal(1, dataReader.GetInt32(0));
        Assert.True(dataReader.Read());
        Assert.Equal(2, dataReader.GetInt32(0));
    }

    [Fact]
    public void SampleTableContainsRow()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT id, target_name, target_int, target_decimal, target_bit FROM sample_table WHERE id = 1;";

        // When
        using var dataReader = command.ExecuteReader();

        // Then
        Assert.True(dataReader.Read());
        Assert.Equal(1, dataReader.GetInt32(0));
        Assert.Equal("table_name", dataReader.GetString(1));
        Assert.Equal(123, dataReader.GetInt32(2));
        Assert.Equal(123.45m, dataReader.GetDecimal(3));
        Assert.True(dataReader.GetBoolean(4));
    }

    [Fact]
    public void MultiTableContainsRows()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM multi_table;";

        // When
        var count = Convert.ToInt32(command.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);

        // Then
        Assert.Equal(14, count);
    }

    [Fact]
    public void PoliciesTableContainsPolicies()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT policy_name FROM policies ORDER BY policy_name;";

        // When
        using var dataReader = command.ExecuteReader();

        // Then
        Assert.True(dataReader.Read());
        Assert.Equal("Admin", dataReader.GetString(0));
        Assert.True(dataReader.Read());
        Assert.Equal("User", dataReader.GetString(0));
    }

    [Fact]
    public void RolePoliciesTableContainsMappings()
    {
        // Given
        using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT role_name, policy_name FROM role_policies ORDER BY role_name, policy_name;";

        // When
        using var dataReader = command.ExecuteReader();

        // Then
        Assert.True(dataReader.Read());
        Assert.Equal("admin", dataReader.GetString(0));
        Assert.Equal("Admin", dataReader.GetString(1));
        Assert.True(dataReader.Read());
        Assert.Equal("admin", dataReader.GetString(0));
        Assert.Equal("User", dataReader.GetString(1));
        Assert.True(dataReader.Read());
        Assert.Equal("user", dataReader.GetString(0));
        Assert.Equal("User", dataReader.GetString(1));
    }
}
