using System;
using System.Collections.Generic;

namespace AspNetCoreSample.Templates.Models;

/// <summary>
/// name
/// </summary>
public partial class Name
{
    /// <summary>
    /// id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名前列
    /// </summary>
    public string Name1 { get; set; } = null!;
}
