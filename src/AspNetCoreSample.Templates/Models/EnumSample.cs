using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.Templates.Models;

[Table("enum_sample")]
public partial class EnumSample
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("enum_column")]
    public int EnumColumn { get; set; }
}
