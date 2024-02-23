using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace CodeGen.Result.Models;

public partial class SampleContext : DbContext
{
    public SampleContext(DbContextOptions<SampleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EnumSample> EnumSamples { get; set; }

    public virtual DbSet<MultiTable> MultiTables { get; set; }

    public virtual DbSet<Name> Names { get; set; }

    public virtual DbSet<SampleTable> SampleTables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<EnumSample>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<MultiTable>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Charid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("multi_table", tb => tb.HasComment("マルチテーブル"));

            entity.Property(e => e.Id).HasComment("マルチID");
            entity.Property(e => e.Charid).HasComment("マルチStringId");
            entity.Property(e => e.CreateAt).HasComment("作成日時");
            entity.Property(e => e.CreateUser).HasComment("作成ユーザー");
            entity.Property(e => e.TargetBit).HasComment("Bit型");
            entity.Property(e => e.TargetDate).HasComment("日付型");
            entity.Property(e => e.TargetDecimal).HasComment("decimal型");
            entity.Property(e => e.TargetInt).HasComment("int型");
            entity.Property(e => e.TargetName).HasComment("名前");
            entity.Property(e => e.UpdateAt).HasComment("更新日時");
            entity.Property(e => e.UpdateUser).HasComment("更新ユーザー");
        });

        modelBuilder.Entity<Name>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SampleTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
