using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.Templates.Models;

[Table("name")]
public partial class Name
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "text")]
    public string Name1 { get; set; } = null!;
}
