using System;
using System.Collections.Generic;

namespace AspNetCoreSample.DataModel.Models;

public partial class RolePolicy
{
    public string PolicyName { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual Policy PolicyNameNavigation { get; set; } = null!;
}
