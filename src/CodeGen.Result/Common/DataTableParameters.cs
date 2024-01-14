using Microsoft.AspNetCore.Mvc;

namespace CodeGen;

[ModelBinder(BinderType = typeof(DataTableModelBinder))]
public class DataTableParameters
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public Search? Search { get; set; }
    public List<ColumnOrder> Order { get; set; } = new();
    public List<Column> Columns { get; set; } = new();
}

public class Search
{
    public string Value { get; set; } = "";
    public bool Regex { get; set; }
}

public class Column
{
    public string Data { get; set; } = "";
    public string Name { get; set; } = "";
    public bool Searchable { get; set; }
    public bool Orderable { get; set; }
    public Search? Search { get; set; }
}

public class ColumnOrder
{
    public int Column { get; set; }
    public ColumnOrderDir Dir { get; set; }
}

public enum ColumnOrderDir
{
    asc,
    dec
}
