using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.DataModel.Models;

public partial class SampleContext : DbContext
{
    public SampleContext(DbContextOptions<SampleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChildTable> ChildTables { get; set; }

    public virtual DbSet<EnumSample> EnumSamples { get; set; }

    public virtual DbSet<MultiTable> MultiTables { get; set; }

    public virtual DbSet<Name> Names { get; set; }

    public virtual DbSet<ParentTable> ParentTables { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<RolePolicy> RolePolicies { get; set; }

    public virtual DbSet<SampleTable> SampleTables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<ChildTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("child_table");

            entity.HasIndex(e => e.ParentId, "child_table_fk");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChildBit)
                .HasColumnType("bit(1)")
                .HasColumnName("child_bit");
            entity.Property(e => e.ChildDate).HasColumnName("child_date");
            entity.Property(e => e.ChildDecimal)
                .HasPrecision(10, 2)
                .HasColumnName("child_decimal");
            entity.Property(e => e.ChildInt).HasColumnName("child_int");
            entity.Property(e => e.ChildName)
                .HasMaxLength(30)
                .HasColumnName("child_name");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasColumnName("create_user");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasColumnName("update_user");

            entity.HasOne(d => d.Parent).WithMany(p => p.ChildTables)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("child_table_fk");
        });

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

            entity.HasIndex(e => e.TargetName, "idx_target_name");

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

        modelBuilder.Entity<ParentTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("parent_table");

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

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyName).HasName("PRIMARY");

            entity.ToTable("policies");

            entity.Property(e => e.PolicyName).HasColumnName("policy_name");
        });

        modelBuilder.Entity<RolePolicy>(entity =>
        {
            entity.HasKey(e => new { e.PolicyName, e.RoleName })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("role_policies");

            entity.Property(e => e.PolicyName).HasColumnName("policy_name");
            entity.Property(e => e.RoleName).HasColumnName("role_name");

            entity.HasOne(d => d.PolicyNameNavigation).WithMany(p => p.RolePolicies)
                .HasForeignKey(d => d.PolicyName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("policies_fk");
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
