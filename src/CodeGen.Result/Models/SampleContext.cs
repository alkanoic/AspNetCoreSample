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
