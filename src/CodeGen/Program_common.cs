using System.Linq;
using System.Text;

using CodeGen;

#pragma warning disable CA1305

public partial class Program
{

    private static void TemplateOutputFile(Type type, object templateArgs, string templateFilePath, string outputPath)
    {
        var templateText = TemplateControl.ReadTemplateText(templateFilePath);
        var tc = new TemplateControl()
        {
            TargetClass = type,
            Instance = templateArgs,
            TemplateText = templateText,
            OutputPath = outputPath
        };
        tc.WriteOverrideText();
    }

    private static void TargetTemplateDirectory(Type type, object templateArgs, string templateDirectory, string outputDirectory, string className)
    {
        foreach (var f in Directory.GetFiles(templateDirectory, "*.*", SearchOption.AllDirectories))
        {
            TemplateOutputFile(type, templateArgs, f, Path.Combine(outputDirectory, f.Replace($"{templateDirectory}/", "").Replace("__ClassName__", className)));
        }
    }
}
