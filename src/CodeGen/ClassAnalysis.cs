using System.Data.Common;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen;

internal sealed class ClassAnalysis(ClassAnalysisArgs args)
{
    public ClassAnalysisInfo ReadCode()
    {
        var code = File.ReadAllText(args.ClassFilePath);
        var tree = CSharpSyntaxTree.ParseText(code);

        var root = tree.GetRoot();
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        ClassDeclarationSyntax @class;
        if (string.IsNullOrEmpty(args.ClassName))
        {
            @class = classes.Single();
        }
        else
        {
            @class = classes.Single(x => x.Identifier.ValueText == args.ClassName);
        }
        var syntaxTree = @class.SyntaxTree;
        var compilationUnit = (CompilationUnitSyntax)syntaxTree.GetRoot();
        var namespaceName = (compilationUnit.Members.Single(x => x.Contains(@class)) as FileScopedNamespaceDeclarationSyntax)?.Name.ToString();

        var classInfo = new ClassAnalysisInfo
        {
            Name = @class.Identifier.ValueText,
            NamespaceName = namespaceName ?? "",
            CommentSummary = GetCommentSummary(@class),
            Attributes = GetAttributeAnalysisInfos(@class.AttributeLists)
        };

        foreach (var p in @class.Members.OfType<PropertyDeclarationSyntax>())
        {
            var pinfo = new PropertyAnalysisInfo()
            {
                Name = p.Identifier.ValueText,
                TypeName = p.Type.ToString()
            };
            if (p.Type is GenericNameSyntax genericType)
            {
                pinfo.GenericTypeName = genericType.Identifier.ValueText;
                pinfo.GenericInnerTypeName = genericType.TypeArgumentList.Arguments[0].ToString();
            }
            pinfo.Attributes = GetAttributeAnalysisInfos(p.AttributeLists);
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

    private static string GetCommentSummary(ClassDeclarationSyntax @class)
    {
        var comment = @class.GetLeadingTrivia().ToFullString().Trim();
        if (string.IsNullOrWhiteSpace(comment)) return "";
        // XML文書としてパース
        var doc = XDocument.Parse(comment.Replace("///", "").Replace("\n", "").Trim());

        // <summary>タグ要素を取得
        return doc?.Root?.Value.Trim() ?? "";
    }

    private static List<AttributeAnalysisInfo> GetAttributeAnalysisInfos(SyntaxList<AttributeListSyntax> attributeLists)
    {
        var attributes = new List<AttributeAnalysisInfo>();
        foreach (var attrs in attributeLists)
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
                attributes.Add(ainfo);
            }
        }
        return attributes;
    }
}
