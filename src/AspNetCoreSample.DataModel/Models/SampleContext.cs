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
        modelBuilder.Entity<ChildTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("child_table_pkc");

            entity.ToTable("child_table", tb => tb.HasComment("child_table"));

            entity.HasIndex(e => e.ParentId, "child_table_fk");

            entity.Property(e => e.Id)
                .HasComment("子テーブルId")
                .HasColumnName("id");
            entity.Property(e => e.ChildBit)
                .HasComment("子テーブルbit型")
                .HasColumnName("child_bit");
            entity.Property(e => e.ChildDate)
                .HasComment("子テーブルDate型")
                .HasColumnName("child_date");
            entity.Property(e => e.ChildDecimal)
                .HasPrecision(10, 2)
                .HasComment("子テーブルdeimal型")
                .HasColumnName("child_decimal");
            entity.Property(e => e.ChildInt)
                .HasComment("子テーブルint型")
                .HasColumnName("child_int");
            entity.Property(e => e.ChildName)
                .HasMaxLength(30)
                .HasComment("子テーブル要素名")
                .HasColumnName("child_name");
            entity.Property(e => e.CreateAt)
                .HasComment("作成日時")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasComment("作成ユーザー")
                .HasColumnName("create_user");
            entity.Property(e => e.ParentId)
                .HasComment("親テーブルid")
                .HasColumnName("parent_id");
            entity.Property(e => e.UpdateAt)
                .HasComment("更新日時")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasComment("更新ユーザー")
                .HasColumnName("update_user");

            entity.HasOne(d => d.Parent).WithMany(p => p.ChildTables)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("child_table_fk1");
        });

        modelBuilder.Entity<EnumSample>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("enum_sample_pkc");

            entity.ToTable("enum_sample", tb => tb.HasComment("enum_sample"));

            entity.Property(e => e.Id)
                .HasComment("id")
                .HasColumnName("id");
            entity.Property(e => e.EnumColumn)
                .HasComment("Enum列")
                .HasColumnName("enum_column");
        });

        modelBuilder.Entity<MultiTable>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Charid }).HasName("multi_table_pkc");

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
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasComment("作成ユーザー")
                .HasColumnName("create_user");
            entity.Property(e => e.TargetBit)
                .HasComment("Bit型")
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
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasComment("更新ユーザー")
                .HasColumnName("update_user");
        });

        modelBuilder.Entity<Name>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("name_pkc");

            entity.ToTable("name", tb => tb.HasComment("name"));

            entity.Property(e => e.Id)
                .HasComment("id")
                .HasColumnName("id");
            entity.Property(e => e.Name1)
                .HasComment("名前列")
                .HasColumnName("name");
        });

        modelBuilder.Entity<ParentTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parent_table_pkc");

            entity.ToTable("parent_table", tb => tb.HasComment("parent_table"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("親テーブルid")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasComment("作成日時")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasComment("作成ユーザー")
                .HasColumnName("create_user");
            entity.Property(e => e.TargetBit)
                .HasComment("親テーブルbit型")
                .HasColumnName("target_bit");
            entity.Property(e => e.TargetDate)
                .HasComment("親テーブルdate型")
                .HasColumnName("target_date");
            entity.Property(e => e.TargetDecimal)
                .HasPrecision(10, 2)
                .HasComment("親テーブルdecimal型")
                .HasColumnName("target_decimal");
            entity.Property(e => e.TargetInt)
                .HasComment("親テーブルint型")
                .HasColumnName("target_int");
            entity.Property(e => e.TargetName)
                .HasMaxLength(30)
                .HasComment("親テーブル要素名")
                .HasColumnName("target_name");
            entity.Property(e => e.UpdateAt)
                .HasComment("更新日時")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasComment("更新ユーザー")
                .HasColumnName("update_user");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyName).HasName("policies_pkc");

            entity.ToTable("policies", tb => tb.HasComment("policies"));

            entity.Property(e => e.PolicyName)
                .HasMaxLength(255)
                .HasComment("ポリシー名")
                .HasColumnName("policy_name");
        });

        modelBuilder.Entity<RolePolicy>(entity =>
        {
            entity.HasKey(e => new { e.RoleName, e.PolicyName }).HasName("role_policies_pkc");

            entity.ToTable("role_policies", tb => tb.HasComment("role_policies"));

            entity.HasIndex(e => e.PolicyName, "policies_fk");

            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasComment("ロール名")
                .HasColumnName("role_name");
            entity.Property(e => e.PolicyName)
                .HasMaxLength(255)
                .HasComment("ポリシー名")
                .HasColumnName("policy_name");

            entity.HasOne(d => d.PolicyNameNavigation).WithMany(p => p.RolePolicies)
                .HasForeignKey(d => d.PolicyName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_policies_fk1");
        });

        modelBuilder.Entity<SampleTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sample_table_pkc");

            entity.ToTable("sample_table", tb => tb.HasComment("sample_table"));

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("id")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasComment("create_at")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(20)
                .HasComment("create_user")
                .HasColumnName("create_user");
            entity.Property(e => e.TargetBit)
                .HasComment("target_bit")
                .HasColumnName("target_bit");
            entity.Property(e => e.TargetDate)
                .HasComment("target_date")
                .HasColumnName("target_date");
            entity.Property(e => e.TargetDecimal)
                .HasPrecision(10, 2)
                .HasComment("target_decimal")
                .HasColumnName("target_decimal");
            entity.Property(e => e.TargetInt)
                .HasComment("target_int")
                .HasColumnName("target_int");
            entity.Property(e => e.TargetName)
                .HasMaxLength(30)
                .HasComment("target_name")
                .HasColumnName("target_name");
            entity.Property(e => e.UpdateAt)
                .HasComment("update_at")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdateUser)
                .HasMaxLength(20)
                .HasComment("update_user")
                .HasColumnName("update_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
