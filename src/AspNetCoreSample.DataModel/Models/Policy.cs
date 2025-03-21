using System;
using System.Collections.Generic;

namespace AspNetCoreSample.DataModel.Models;

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
