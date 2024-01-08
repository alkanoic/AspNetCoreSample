using System.Globalization;
using System.Text;

namespace CodeGen;

internal sealed class TemplateControl
{
    private const string ReplaceChara = "__";

    public required Type TargetClass { get; set; }

    public required object Instance { get; set; }

    public required string TemplateText { get; set; }

    public required string OutputPath { get; set; }

    private Dictionary<string, string> ClassPropertiesConvertToDictionary()
    {
        var dictionary = new Dictionary<string, string>();

        foreach (var p in TargetClass.GetProperties())
        {
            var v = p.GetValue(Instance);
            dictionary.Add($"{ReplaceChara}{p.Name}{ReplaceChara}", v is null ? "" : v.ToString()!);
        }
        return dictionary;
    }

    private string TemplateOverride()
    {
        var dict = ClassPropertiesConvertToDictionary();
        var outputText = TemplateText;
        foreach (var key in dict.Keys)
        {
            outputText = outputText.Replace(key, dict[key]);
        }
        return outputText;
    }

    public void WriteOverrideText()
    {
        var text = TemplateOverride();
        var directoryName = Path.GetDirectoryName(OutputPath);
        if (directoryName is null)
        {
            throw new DirectoryNotFoundException($"OutputPath: {OutputPath}");
        }
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        File.WriteAllText(OutputPath, text, new UTF8Encoding(false));
    }

    public static string ReadTemplateText(string templateFileName)
    {
        string appPath = AppContext.BaseDirectory;
        return File.ReadAllText(Path.Combine(appPath, $"Templates/{templateFileName}"),
            new UTF8Encoding(false));
    }

    public static string UsingNamespace(string namespaceName)
    {
        return $"using {namespaceName};";
    }

    public static string AuthorizeAttribute(bool authorize)
    {
        return authorize ? "[Authorize]" : "";
    }

    public static string CompareTargetToArguments(List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        foreach (var pp in primaryProperties)
        {
            target.Append(DefaultCultureInfo, $"target.{pp.Name} != {pp.Name.ToCamelCase()} || ");
        }
        return target.ToString().LastCharRemove(4);
    }

    public static string PrimaryKeyArguments(List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        foreach (var pp in primaryProperties)
        {
            target.Append(DefaultCultureInfo, $"{pp.TypeName} {pp.Name.ToCamelCase()},");
        }
        return target.ToString().LastCharRemove();
    }

    public static string PrimaryKeyNameAttributes(List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        foreach (var pp in primaryProperties)
        {
            target.Append(DefaultCultureInfo, $"{pp.Name.ToCamelCase()}/");
        }
        return target.ToString().LastCharRemove();
    }

    public static string PrimaryKeyNameArguments(List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        foreach (var pp in primaryProperties)
        {
            target.Append(DefaultCultureInfo, $"{pp.Name.ToCamelCase()},");
        }
        return target.ToString().LastCharRemove();
    }

    public static string PrimaryKeyNameTargetArguments(List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        foreach (var pp in primaryProperties)
        {
            target.Append(DefaultCultureInfo, $"target.{pp.Name}, ");
        }
        return target.ToString().LastCharRemove(2);
    }

    public static string PrimaryKeyNameNewObject(List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        foreach (var pp in primaryProperties)
        {
            target.Append(DefaultCultureInfo, $"{pp.Name.ToCamelCase()} = target.{pp.Name}, ");
        }
        return target.ToString().LastCharRemove(2);
    }

    public static string ContextFindPrimaryKey(string entitySetName, List<PropertyAnalysisInfo> primaryProperties, bool asNoTracking)
    {
        var target = new StringBuilder();
        if (asNoTracking)
        {
            target.AppendLine(DefaultCultureInfo, $"var target = _context.{entitySetName}.AsNoTracking();");
        }
        else
        {
            target.AppendLine(DefaultCultureInfo, $"var target = _context.{entitySetName};");
        }
        foreach (var pp in primaryProperties)
        {
            target.AppendLine(DefaultCultureInfo, $"target = target.Where(x => x.{pp.Name} == {pp.Name.ToCamelCase()});");
        }
        target.AppendLine(DefaultCultureInfo, $"var result = await target.SingleOrDefaultAsync();");
        return target.ToString();
    }

    public static string EntitySetExist(string entitySetName, List<PropertyAnalysisInfo> primaryProperties)
    {
        var target = new StringBuilder();
        target.AppendLine(DefaultCultureInfo, $"var target = _context.{entitySetName}.AsNoTracking();");
        foreach (var pp in primaryProperties)
        {
            target.AppendLine(DefaultCultureInfo, $"target = target.Where(x => x.{pp.Name} == {pp.Name.ToCamelCase()});");
        }
        target.Append(DefaultCultureInfo, $"return target.Any();");
        return target.ToString();
    }

    private static CultureInfo DefaultCultureInfo = CultureInfo.InvariantCulture;
}
