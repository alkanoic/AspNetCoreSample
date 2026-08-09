using AspNetCoreSample.DataModel.Models;

using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.DataModel.Test;

public sealed class SampleContextTest : IDisposable
{
    private readonly SampleContext _context;

    public SampleContextTest()
    {
        var options = new DbContextOptionsBuilder<SampleContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new SampleContext(options);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task AddAndRetrieveName()
    {
        var name = new Name { Id = 0, Name1 = "テスト" };
        _context.Names.Add(name);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.Names.FirstOrDefaultAsync(n => n.Name1 == "テスト", TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("テスト", result.Name1);
        Assert.True(result.Id > 0);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task NamesDbSetIsNotNull()
    {
        var names = await _context.Names.ToListAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(names);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task AddAndRetrieveSampleTable()
    {
        var sample = new SampleTable
        {
            Id = 1,
            TargetName = "サンプル",
            TargetInt = 100,
            TargetDecimal = 99.99m,
            TargetDate = new DateOnly(2024, 1, 1),
            TargetBit = true,
            CreateAt = DateTime.UtcNow,
            CreateUser = "test",
            UpdateAt = DateTime.UtcNow,
            UpdateUser = "test"
        };
        _context.SampleTables.Add(sample);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.SampleTables.FindAsync([1], TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("サンプル", result.TargetName);
        Assert.Equal(100, result.TargetInt);
        Assert.Equal(99.99m, result.TargetDecimal);
        Assert.True(result.TargetBit);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task AddAndRetrieveParentWithChildren()
    {
        var parent = new ParentTable
        {
            Id = 1,
            TargetName = "親テーブル",
            CreateAt = DateTime.UtcNow,
            CreateUser = "test",
            UpdateAt = DateTime.UtcNow,
            UpdateUser = "test"
        };
        var child = new ChildTable
        {
            Id = 0,
            ParentId = 1,
            ChildName = "子テーブル",
            CreateAt = DateTime.UtcNow,
            CreateUser = "test",
            UpdateAt = DateTime.UtcNow,
            UpdateUser = "test"
        };
        parent.ChildTables.Add(child);
        _context.ParentTables.Add(parent);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.ParentTables.Include(p => p.ChildTables).FirstOrDefaultAsync(p => p.Id == 1, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Single(result.ChildTables);
        Assert.Equal("子テーブル", result.ChildTables.First().ChildName);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task AddAndRetrieveMultiTable()
    {
        var multi = new MultiTable
        {
            Id = 1,
            Charid = "KEY001",
            TargetName = "マルチ",
            CreateAt = DateTime.UtcNow,
            CreateUser = "test",
            UpdateAt = DateTime.UtcNow,
            UpdateUser = "test"
        };
        _context.MultiTables.Add(multi);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.MultiTables.FindAsync([1, "KEY001"], TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("マルチ", result.TargetName);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task AddAndRetrieveEnumSample()
    {
        var enumSample = new EnumSample { Id = 0, EnumColumn = 1 };
        _context.EnumSamples.Add(enumSample);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.EnumSamples.FirstOrDefaultAsync(e => e.EnumColumn == 1, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(1, result.EnumColumn);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task AddAndRetrievePolicyWithRolePolicies()
    {
        var policy = new Policy { PolicyName = "AdminPolicy" };
        var rolePolicy = new RolePolicy { RoleName = "Admin", PolicyName = "AdminPolicy" };
        policy.RolePolicies.Add(rolePolicy);
        _context.Policies.Add(policy);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.Policies.Include(p => p.RolePolicies).FirstOrDefaultAsync(p => p.PolicyName == "AdminPolicy", TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Single(result.RolePolicies);
        Assert.Equal("Admin", result.RolePolicies.First().RoleName);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task UpdateName()
    {
        var name = new Name { Id = 0, Name1 = "更新前" };
        _context.Names.Add(name);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        name.Name1 = "更新後";
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.Names.FindAsync([name.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal("更新後", result.Name1);
    }

    [Fact]
    [Trait("Category", nameof(SampleContextTest))]
    public async Task DeleteName()
    {
        var name = new Name { Id = 0, Name1 = "削除対象" };
        _context.Names.Add(name);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _context.Names.Remove(name);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _context.Names.FindAsync([name.Id], TestContext.Current.CancellationToken);
        Assert.Null(result);
    }
}
