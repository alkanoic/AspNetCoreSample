using System;
using System.Collections.Generic;

namespace AspNetCoreSample.Templates.Models;

/// <summary>
/// マルチテーブル
/// </summary>
public partial class MultiTable
{
    /// <summary>
    /// マルチID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// マルチStringId
    /// </summary>
    public string Charid { get; set; } = null!;

    /// <summary>
    /// 名前
    /// </summary>
    public string TargetName { get; set; } = null!;

    /// <summary>
    /// int型
    /// </summary>
    public int? TargetInt { get; set; }

    /// <summary>
    /// decimal型
    /// </summary>
    public decimal? TargetDecimal { get; set; }

    /// <summary>
    /// 日付型
    /// </summary>
    public DateOnly? TargetDate { get; set; }

    /// <summary>
    /// Bit型
    /// </summary>
    public bool? TargetBit { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreateAt { get; set; }

    /// <summary>
    /// 作成ユーザー
    /// </summary>
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime UpdateAt { get; set; }

    /// <summary>
    /// 更新ユーザー
    /// </summary>
    public string UpdateUser { get; set; } = null!;
}
