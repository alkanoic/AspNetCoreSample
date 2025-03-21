using System;
using System.Collections.Generic;

namespace AspNetCoreSample.DataModel.Models;

/// <summary>
/// role_policies
/// </summary>
public partial class RolePolicy
{
    /// <summary>
    /// ロール名
    /// </summary>
    public string RoleName { get; set; } = null!;

    /// <summary>
    /// ポリシー名
    /// </summary>
    public string PolicyName { get; set; } = null!;

    public virtual Policy PolicyNameNavigation { get; set; } = null!;
}
