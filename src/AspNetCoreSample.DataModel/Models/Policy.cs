using System;
using System.Collections.Generic;

namespace AspNetCoreSample.DataModel.Models;

public partial class Policy
{
    public string PolicyName { get; set; } = null!;

    public virtual ICollection<RolePolicy> RolePolicies { get; set; } = new List<RolePolicy>();
}
