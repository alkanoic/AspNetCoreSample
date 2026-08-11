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

    [Test]
    [Category(nameof(SampleContextTest))]
    public async Task AddAndRetrieveName()
    {
        var name = new Name { Id = 0, Name1 = "テスト" };
        _context.Names.Add(name);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.Names.FirstOrDefaultAsync(n => n.Name1 == "テスト", CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Name1).IsEqualTo("テスト");
        await Assert.That(result.Id > 0).IsTrue();
    }

    [Test]
    [Category(nameof(SampleContextTest))]
    public async Task NamesDbSetIsNotNull()
    {
        var names = await _context.Names.ToListAsync(CancellationToken.None);
        await Assert.That(names).IsNotNull();
    }

    [Test]
    [Category(nameof(SampleContextTest))]
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
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.SampleTables.FindAsync([1], CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TargetName).IsEqualTo("サンプル");
        await Assert.That(result.TargetInt).IsEqualTo(100);
        await Assert.That(result.TargetDecimal).IsEqualTo(99.99m);
        await Assert.That(result.TargetBit).IsTrue();
    }

    [Test]
    [Category(nameof(SampleContextTest))]
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
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.ParentTables.Include(p => p.ChildTables).FirstOrDefaultAsync(p => p.Id == 1, CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ChildTables).HasSingleItem();
        await Assert.That(result.ChildTables.First().ChildName).IsEqualTo("子テーブル");
    }

    [Test]
    [Category(nameof(SampleContextTest))]
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
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.MultiTables.FindAsync([1, "KEY001"], CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TargetName).IsEqualTo("マルチ");
    }

    [Test]
    [Category(nameof(SampleContextTest))]
    public async Task AddAndRetrieveEnumSample()
    {
        var enumSample = new EnumSample { Id = 0, EnumColumn = 1 };
        _context.EnumSamples.Add(enumSample);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.EnumSamples.FirstOrDefaultAsync(e => e.EnumColumn == 1, CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.EnumColumn).IsEqualTo(1);
    }

    [Test]
    [Category(nameof(SampleContextTest))]
    public async Task AddAndRetrievePolicyWithRolePolicies()
    {
        var policy = new Policy { PolicyName = "AdminPolicy" };
        var rolePolicy = new RolePolicy { RoleName = "Admin", PolicyName = "AdminPolicy" };
        policy.RolePolicies.Add(rolePolicy);
        _context.Policies.Add(policy);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.Policies.Include(p => p.RolePolicies).FirstOrDefaultAsync(p => p.PolicyName == "AdminPolicy", CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.RolePolicies).HasSingleItem();
        await Assert.That(result.RolePolicies.First().RoleName).IsEqualTo("Admin");
    }

    [Test]
    [Category(nameof(SampleContextTest))]
    public async Task UpdateName()
    {
        var name = new Name { Id = 0, Name1 = "更新前" };
        _context.Names.Add(name);
        await _context.SaveChangesAsync(CancellationToken.None);

        name.Name1 = "更新後";
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.Names.FindAsync([name.Id], CancellationToken.None);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Name1).IsEqualTo("更新後");
    }

    [Test]
    [Category(nameof(SampleContextTest))]
    public async Task DeleteName()
    {
        var name = new Name { Id = 0, Name1 = "削除対象" };
        _context.Names.Add(name);
        await _context.SaveChangesAsync(CancellationToken.None);

        _context.Names.Remove(name);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _context.Names.FindAsync([name.Id], CancellationToken.None);
        await Assert.That(result).IsNull();
    }
}
