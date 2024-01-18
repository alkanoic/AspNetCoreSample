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
    public SearchBuilder SearchBuilder { get; set; } = new();
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

public class SearchBuilder
{
    public SearchLogic Logic { get; set; }

    public List<SearchCondition> SearchConditions { get; set; } = new();

    public static SearchLogic ToSearchLogic(string value)
    {
        switch (value)
        {
            case "AND":
                return SearchLogic.AND;
            case "OR":
                return SearchLogic.OR;
            default: return SearchLogic.AND;
        }
    }
}

public class SearchCondition
{
    public SearchNumCondition NumCondition { get; set; }

    public SearchStrCondition StrCondition { get; set; }

    public string Data { get; set; } = "";

    public string OriginalData { get; set; } = "";

    public SearchColumnType Type { get; set; }

    public string Value1 { get; set; } = "";

    public string Value2 { get; set; } = "";

    public static SearchNumCondition ToSearchNumCondition(string value)
    {
        switch (value)
        {
            case "=":
                return SearchNumCondition.Equals;
            case "!=":
                return SearchNumCondition.Not;
            case "<":
                return SearchNumCondition.LessThan;
            case "<=":
                return SearchNumCondition.LessThanEqualTo;
            case ">=":
                return SearchNumCondition.GreaterThanEqualTo;
            case ">":
                return SearchNumCondition.GreaterThan;
            case "between":
                return SearchNumCondition.Between;
            case "!between":
                return SearchNumCondition.NotBetween;
            case "null":
                return SearchNumCondition.Empty;
            case "!null":
                return SearchNumCondition.NotEmpty;
            default: return SearchNumCondition.Equals;
        }
    }

    public static SearchStrCondition ToSearchStrCondition(string value)
    {
        switch (value)
        {
            case "=":
                return SearchStrCondition.Equals;
            case "!=":
                return SearchStrCondition.Not;
            case "starts":
                return SearchStrCondition.StartsWith;
            case "!starts":
                return SearchStrCondition.DoesNotStartWith;
            case "contains":
                return SearchStrCondition.Contains;
            case "!contains":
                return SearchStrCondition.DoesNotContain;
            case "ends":
                return SearchStrCondition.EndsWith;
            case "!ends":
                return SearchStrCondition.DoesNotEndWith;
            case "null":
                return SearchStrCondition.Empty;
            case "!null":
                return SearchStrCondition.NotEmpty;
            default: return SearchStrCondition.Equals;
        }
    }

    public static SearchColumnType ToSearchColumnType(string value)
    {
        switch (value)
        {
            case "num":
                return SearchColumnType.Num;
            case "string":
                return SearchColumnType.Str;
            default: return SearchColumnType.Num;
        }
    }
}

public enum SearchNumCondition
{
    Equals,
    Not,
    LessThan,
    LessThanEqualTo,
    GreaterThanEqualTo,
    GreaterThan,
    Between,
    NotBetween,
    Empty,
    NotEmpty
}

public enum SearchStrCondition
{
    Equals,
    Not,
    StartsWith,
    DoesNotStartWith,
    Contains,
    DoesNotContain,
    EndsWith,
    DoesNotEndWith,
    Empty,
    NotEmpty
}

public enum SearchColumnType
{
    Num, Str
}

public enum SearchLogic
{
    AND, OR
}
