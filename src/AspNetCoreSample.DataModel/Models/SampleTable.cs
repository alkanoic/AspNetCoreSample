using System;
using System.Collections.Generic;

namespace AspNetCoreSample.DataModel.Models;

/// <summary>
/// sample_table
/// </summary>
public partial class SampleTable
{
    /// <summary>
    /// id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// target_name
    /// </summary>
    public string TargetName { get; set; } = null!;

    /// <summary>
    /// target_int
    /// </summary>
    public int? TargetInt { get; set; }

    /// <summary>
    /// target_decimal
    /// </summary>
    public decimal? TargetDecimal { get; set; }

    /// <summary>
    /// target_date
    /// </summary>
    public DateOnly? TargetDate { get; set; }

    /// <summary>
    /// target_bit
    /// </summary>
    public bool? TargetBit { get; set; }

    /// <summary>
    /// create_at
    /// </summary>
    public DateTime CreateAt { get; set; }

    /// <summary>
    /// create_user
    /// </summary>
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// update_at
    /// </summary>
    public DateTime UpdateAt { get; set; }

    /// <summary>
    /// update_user
    /// </summary>
    public string UpdateUser { get; set; } = null!;
}
