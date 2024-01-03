using System.Data.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen;

public class ClassAnalysis(ClassAnalysisArgs args)
{
    public ClassAnalysisInfo ReadCode()
    {
        var code = File.ReadAllText(args.ClassFilePath);
        var tree = CSharpSyntaxTree.ParseText(code);

        var root = tree.GetRoot();
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        var @class = classes.Single(x => x.Identifier.ValueText == args.ClassName);
        var classInfo = new ClassAnalysisInfo()
        {
            Name = @class.Identifier.ValueText
        };

        foreach (var p in @class.Members.OfType<PropertyDeclarationSyntax>())
        {
            var pinfo = new PropertyAnalysisInfo()
            {
                Name = p.Identifier.ValueText,
                TypeName = p.Type.ToString()
            };
            foreach (var attrs in p.AttributeLists)
            {
                foreach (var attr in attrs.Attributes)
                {
                    var ainfo = new AttributeAnalysisInfo()
                    {
                        Name = attr.Name.ToString()
                    };
                    if (attr.ArgumentList is not null)
                    {
                        foreach (var a in attr.ArgumentList.Arguments)
                        {
                            var arinfo = new ArgumentAnalysisInfo()
                            {
                                Name = a.NameEquals?.Name.ToString() ?? "",
                                Value = a.Expression.ToString().Replace("\"", "")
                            };
                            ainfo.Arguments.Add(arinfo);
                        }
                    }
                    pinfo.Attributes.Add(ainfo);
                }
            }
            classInfo.Properties.Add(pinfo);
        }

        foreach (var m in @class.Members.OfType<MethodDeclarationSyntax>())
        {
            var minfo = new MethodAnalysisInfo()
            {
                Name = m.Identifier.ValueText
            };
            classInfo.Methods.Add(minfo);
        }

        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(classInfo));

        return classInfo;
    }
}
