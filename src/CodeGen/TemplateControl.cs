using System.Text;

namespace CodeGen;

public class TemplateControl
{
    private const string ReplaceChara = "@";

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
        File.WriteAllText(OutputPath, text, new UTF8Encoding(false));
    }
}
