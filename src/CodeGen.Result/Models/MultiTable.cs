using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace CodeGen.Result.Models;

/// <summary>
/// マルチテーブル
/// </summary>
[PrimaryKey("Id", "Charid")]
[Table("multi_table")]
public partial class MultiTable
{
    /// <summary>
    /// マルチID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// マルチStringId
    /// </summary>
    [Key]
    [Column("charid")]
    [StringLength(10)]
    public string Charid { get; set; } = null!;

    /// <summary>
    /// 名前
    /// </summary>
    [Column("target_name")]
    [StringLength(30)]
    public string TargetName { get; set; } = null!;

    /// <summary>
    /// int型
    /// </summary>
    [Column("target_int")]
    public int? TargetInt { get; set; }

    /// <summary>
    /// decimal型
    /// </summary>
    [Column("target_decimal")]
    [Precision(10, 2)]
    public decimal? TargetDecimal { get; set; }

    /// <summary>
    /// 日付型
    /// </summary>
    [Column("target_date")]
    public DateOnly? TargetDate { get; set; }

    /// <summary>
    /// Bit型
    /// </summary>
    [Column("target_bit", TypeName = "bit(1)")]
    public ulong? TargetBit { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    [Column("create_at", TypeName = "datetime")]
    public DateTime CreateAt { get; set; }

    /// <summary>
    /// 作成ユーザー
    /// </summary>
    [Column("create_user")]
    [StringLength(20)]
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// 更新日時
    /// </summary>
    [Column("update_at", TypeName = "datetime")]
    public DateTime UpdateAt { get; set; }

    /// <summary>
    /// 更新ユーザー
    /// </summary>
    [Column("update_user")]
    [StringLength(20)]
    public string UpdateUser { get; set; } = null!;
}
