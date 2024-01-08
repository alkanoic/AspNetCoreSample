using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.Templates.Models;

[Table("sample_table")]
public partial class SampleTable
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("target_name")]
    [StringLength(30)]
    public string TargetName { get; set; } = null!;

    [Column("target_int")]
    public int? TargetInt { get; set; }

    [Column("target_decimal")]
    [Precision(10, 2)]
    public decimal? TargetDecimal { get; set; }

    [Column("target_date")]
    public DateOnly? TargetDate { get; set; }

    [Column("target_bit", TypeName = "bit(1)")]
    public ulong? TargetBit { get; set; }

    [Column("create_at", TypeName = "datetime")]
    public DateTime CreateAt { get; set; }

    [Column("create_user")]
    [StringLength(20)]
    public string CreateUser { get; set; } = null!;

    [Column("update_at", TypeName = "datetime")]
    public DateTime UpdateAt { get; set; }

    [Column("update_user")]
    [StringLength(20)]
    public string UpdateUser { get; set; } = null!;
}
