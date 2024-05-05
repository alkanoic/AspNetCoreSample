using System;
using System.Collections.Generic;

namespace AspNetCoreSample.WebApi.EfModels;

public partial class SampleTable
{
    public int Id { get; set; }

    public string TargetName { get; set; } = null!;

    public int? TargetInt { get; set; }

    public decimal? TargetDecimal { get; set; }

    public DateOnly? TargetDate { get; set; }

    public ulong? TargetBit { get; set; }

    public DateTime CreateAt { get; set; }

    public string CreateUser { get; set; } = null!;

    public DateTime UpdateAt { get; set; }

    public string UpdateUser { get; set; } = null!;
}
