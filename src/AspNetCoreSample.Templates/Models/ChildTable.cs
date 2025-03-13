using System;
using System.Collections.Generic;

namespace AspNetCoreSample.Templates.Models;

/// <summary>
/// child_table
/// </summary>
public partial class ChildTable
{
    /// <summary>
    /// 子テーブルId
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 親テーブルid
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// 子テーブル要素名
    /// </summary>
    public string ChildName { get; set; } = null!;

    /// <summary>
    /// 子テーブルint型
    /// </summary>
    public int? ChildInt { get; set; }

    /// <summary>
    /// 子テーブルdeimal型
    /// </summary>
    public decimal? ChildDecimal { get; set; }

    /// <summary>
    /// 子テーブルDate型
    /// </summary>
    public DateOnly? ChildDate { get; set; }

    /// <summary>
    /// 子テーブルbit型
    /// </summary>
    public bool? ChildBit { get; set; }

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

    public virtual ParentTable Parent { get; set; } = null!;
}
