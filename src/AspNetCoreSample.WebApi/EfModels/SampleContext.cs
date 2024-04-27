using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.WebApi.EfModels;

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

            entity.ToTable("enum_sample");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EnumColumn).HasColumnName("enum_column");
        });

        modelBuilder.Entity<MultiTable>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Charid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("multi_table", tb => tb.HasComment("マルチテーブル"));

            entity.Property(e => e.Id)
                .HasComment("マルチID")
                .HasColumnName("id");
            entity.Property(e => e.Charid)
                .HasMaxLength(10)
                .HasComment("マルチStringId")
                .HasColumnName("charid");
            entity.Property(e => e.CreateAt)
                .HasComment("作成日時")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasComment("作成ユーザー")
                .HasColumnName("create_user");
            entity.Property(e => e.TargetBit)
                .HasComment("Bit型")
                .HasColumnType("bit(1)")
                .HasColumnName("target_bit");
            entity.Property(e => e.TargetDate)
                .HasComment("日付型")
                .HasColumnName("target_date");
            entity.Property(e => e.TargetDecimal)
                .HasPrecision(10, 2)
                .HasComment("decimal型")
                .HasColumnName("target_decimal");
            entity.Property(e => e.TargetInt)
                .HasComment("int型")
                .HasColumnName("target_int");
            entity.Property(e => e.TargetName)
                .HasMaxLength(30)
                .HasComment("名前")
                .HasColumnName("target_name");
            entity.Property(e => e.UpdateAt)
                .HasComment("更新日時")
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasComment("更新ユーザー")
                .HasColumnName("update_user");
        });

        modelBuilder.Entity<Name>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("name");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name1)
                .HasColumnType("text")
                .HasColumnName("name");
        });

        modelBuilder.Entity<SampleTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sample_table");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasColumnName("create_user");
            entity.Property(e => e.TargetBit)
                .HasColumnType("bit(1)")
                .HasColumnName("target_bit");
            entity.Property(e => e.TargetDate).HasColumnName("target_date");
            entity.Property(e => e.TargetDecimal)
                .HasPrecision(10, 2)
                .HasColumnName("target_decimal");
            entity.Property(e => e.TargetInt).HasColumnName("target_int");
            entity.Property(e => e.TargetName)
                .HasMaxLength(30)
                .HasColumnName("target_name");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasColumnName("update_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
