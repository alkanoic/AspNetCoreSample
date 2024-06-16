using System;
using System.Collections.Generic;

namespace AspNetCoreSample.DataModel.Models;

public partial class ChildTable
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string ChildName { get; set; } = null!;

    public int? ChildInt { get; set; }

    public decimal? ChildDecimal { get; set; }

    public DateOnly? ChildDate { get; set; }

    public ulong? ChildBit { get; set; }

    public DateTime CreateAt { get; set; }

    public string CreateUser { get; set; } = null!;

    public DateTime UpdateAt { get; set; }

    public string UpdateUser { get; set; } = null!;

    public virtual ParentTable Parent { get; set; } = null!;
}
