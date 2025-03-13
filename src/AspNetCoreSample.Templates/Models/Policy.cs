using System;
using System.Collections.Generic;

namespace AspNetCoreSample.Templates.Models;

/// <summary>
/// policies
/// </summary>
public partial class Policy
{
    /// <summary>
    /// ポリシー名
    /// </summary>
    public string PolicyName { get; set; } = null!;

    public virtual ICollection<RolePolicy> RolePolicies { get; set; } = new List<RolePolicy>();
}
