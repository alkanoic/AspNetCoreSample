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
}
