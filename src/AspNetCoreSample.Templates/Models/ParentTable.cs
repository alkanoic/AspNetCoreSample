using System;
using System.Collections.Generic;

namespace AspNetCoreSample.Templates.Models;

/// <summary>
/// parent_table
/// </summary>
public partial class ParentTable
{
    /// <summary>
    /// 親テーブルid
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 親テーブル要素名
    /// </summary>
    public string TargetName { get; set; } = null!;

    /// <summary>
    /// 親テーブルint型
    /// </summary>
    public int? TargetInt { get; set; }

    /// <summary>
    /// 親テーブルdecimal型
    /// </summary>
    public decimal? TargetDecimal { get; set; }

    /// <summary>
    /// 親テーブルdate型
    /// </summary>
    public DateOnly? TargetDate { get; set; }

    /// <summary>
    /// 親テーブルbit型
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

    public virtual ICollection<ChildTable> ChildTables { get; set; } = new List<ChildTable>();
}
