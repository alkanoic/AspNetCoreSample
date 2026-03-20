#!/usr/bin/env dotnet-script
// InlineExpander.csx
// 使い方:
//   dotnet script InlineExpander.csx -- --solution "C:\path\to\MyApp.sln" --apikey "sk-ant-..."
//
// 必要パッケージ（自動インストール）:
#r "nuget: Microsoft.CodeAnalysis.CSharp, 4.8.0"
#r "nuget: System.Net.Http, 4.3.4"

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ============================================================
// 設定
// ============================================================
var args = Args.ToList();
string solutionPath = GetArg(args, "--solution") ?? throw new Exception("--solution を指定してください");
string apiKey = GetArg(args, "--apikey") ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
string outputDir = GetArg(args, "--output") ?? Path.Combine(Path.GetDirectoryName(solutionPath)!, "form_analysis");
int maxDepth = int.Parse(GetArg(args, "--depth") ?? "10");
bool skipApi = string.IsNullOrEmpty(apiKey);

Directory.CreateDirectory(Path.Combine(outputDir, "expanded"));
Directory.CreateDirectory(Path.Combine(outputDir, "features"));

Console.WriteLine($"[設定] ソリューション : {solutionPath}");
Console.WriteLine($"[設定] 出力先         : {outputDir}");
Console.WriteLine($"[設定] API呼び出し    : {(skipApi ? "スキップ（--apikey 未指定）" : "有効")}");
Console.WriteLine();

// ============================================================
// ソリューションから .cs ファイルを収集
// ============================================================
Console.WriteLine("[1/4] ソリューションを読み込み中...");

var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionPath))!;
var csFiles = new List<string>();

// .sln からプロジェクトパスを抽出
var slnContent = await File.ReadAllTextAsync(solutionPath);
var projPathPattern = new Regex(@"Project\(""\{[^}]+\}""\)\s*=\s*""[^""]*""\s*,\s*""([^""]+\.csproj)""");
foreach (Match m in projPathPattern.Matches(slnContent))
{
    var relPath = m.Groups[1].Value.Replace('\\', Path.DirectorySeparatorChar);
    var projPath = Path.GetFullPath(Path.Combine(solutionDir, relPath));
    var projDir = Path.GetDirectoryName(projPath)!;
    if (Directory.Exists(projDir))
        csFiles.AddRange(Directory.EnumerateFiles(projDir, "*.cs", SearchOption.AllDirectories));
}

Console.WriteLine($"       {csFiles.Count} .cs ファイルを発見");

// ============================================================
// 全ファイルを構文解析してメソッドをキャッシュ
// ============================================================
Console.WriteLine("[2/4] プロジェクト全体を解析中...");

// key: "ClassName.MethodName" -> MethodDeclarationSyntax
var methodMap = new Dictionary<string, MethodDeclarationSyntax>();
// key: ファイルパス -> SyntaxTree
var treeByFile = new Dictionary<string, SyntaxTree>();

foreach (var file in csFiles)
{
    var src = await File.ReadAllTextAsync(file);
    var tree = CSharpSyntaxTree.ParseText(src, path: file);
    treeByFile[file] = tree;
    var root = await tree.GetRootAsync();
    foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
    {
        var className = (method.Parent as ClassDeclarationSyntax)?.Identifier.Text ?? "";
        var key = $"{className}.{method.Identifier.Text}";
        if (!methodMap.ContainsKey(key))
            methodMap[key] = method;
    }
}
Console.WriteLine($"       {methodMap.Count} メソッドをキャッシュ");

// ============================================================
// 各Formのイベントハンドラを収集して展開
// ============================================================
Console.WriteLine("[3/4] Formイベントハンドラを展開中...");

var eventHandlerPattern = new Regex(@"_(Click|Load|Changed|Enter|Leave|KeyDown|KeyUp|KeyPress|DoubleClick|MouseDown|MouseUp|FormClosing|FormClosed|Shown|Resize|SelectedIndexChanged|TextChanged|CheckedChanged|ValueChanged|CellClick|CellValueChanged|RowEnter|RowLeave|SelectionChanged|AfterSelect|NodeMouseClick|Paint|Validated|Validating|DropDown|DropDownClosed|ItemActivate|ColumnClick|DrawItem|MeasureItem|Scroll|Tick)$");

var formResults = new List<FormAnalysisResult>();

foreach (var (filePath, tree) in treeByFile)
{
    if (filePath.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)) continue;

    var root = await tree.GetRootAsync();

    foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
    {
        if (!InheritsFromForm(classDecl)) continue;

        var formName = classDecl.Identifier.Text;
        Console.WriteLine($"       Form発見: {formName}");

        var formResult = new FormAnalysisResult { FormName = formName, FilePath = filePath };

        foreach (var method in classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (!IsEventHandler(method, eventHandlerPattern)) continue;

            var methodName = method.Identifier.Text;
            Console.WriteLine($"         イベント: {methodName}");

            var sb = new StringBuilder();
            var visited = new HashSet<string>();
            sb.AppendLine($"// === {formName}.{methodName} ===");
            sb.AppendLine($"// ファイル: {filePath}");
            sb.AppendLine();
            ExpandMethod(method, formName, sb, visited, 0, maxDepth, "");

            var expandedCode = sb.ToString();
            var safeFormName = SanitizeName(formName);
            var safeEventName = SanitizeName(methodName);
            var expandedDir = Path.Combine(outputDir, "expanded", safeFormName);
            Directory.CreateDirectory(expandedDir);
            var expandedFile = Path.Combine(expandedDir, $"{safeEventName}.txt");
            await File.WriteAllTextAsync(expandedFile, expandedCode, Encoding.UTF8);

            formResult.Events.Add(new EventExpansion
            {
                EventName = methodName,
                ExpandedFile = expandedFile,
                ExpandedCode = expandedCode
            });
        }

        if (formResult.Events.Count > 0)
            formResults.Add(formResult);
    }
}

Console.WriteLine($"       {formResults.Count} Form, {formResults.Sum(f => f.Events.Count)} イベントを展開");

// ============================================================
// LLM API で機能分析
// ============================================================
Console.WriteLine("[4/4] 機能分析中...");

foreach (var form in formResults)
{
    var featureFile = Path.Combine(outputDir, "features", $"{SanitizeName(form.FormName)}.md");

    if (skipApi)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {form.FormName} - 展開済みイベントコード");
        sb.AppendLine($"> API未設定のため機能分析はスキップされました。`--apikey` を指定して再実行してください。");
        sb.AppendLine();
        foreach (var ev in form.Events)
        {
            sb.AppendLine($"## {ev.EventName}");
            sb.AppendLine("```csharp");
            sb.AppendLine(ev.ExpandedCode);
            sb.AppendLine("```");
            sb.AppendLine();
        }
        await File.WriteAllTextAsync(featureFile, sb.ToString(), Encoding.UTF8);
        Console.WriteLine($"       {form.FormName}: スキップ → {featureFile}");
        continue;
    }

    var prompt = BuildAnalysisPrompt(form);
    try
    {
        var result = await CallClaudeApi(apiKey, prompt);
        await File.WriteAllTextAsync(featureFile, result, Encoding.UTF8);
        Console.WriteLine($"       {form.FormName}: 完了 → {featureFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"       {form.FormName}: API呼び出し失敗 - {ex.Message}");
        await File.WriteAllTextAsync(featureFile, $"# エラー\n{ex.Message}\n\n## プロンプト\n{prompt}", Encoding.UTF8);
    }

    await Task.Delay(1000);
}

Console.WriteLine();
Console.WriteLine($"完了！出力先: {outputDir}");
Console.WriteLine($"  expanded/ ... 展開済みコード（Form/イベント別）");
Console.WriteLine($"  features/ ... 機能分析結果（Form別Markdown）");

// ============================================================
// ヘルパー関数
// ============================================================

void ExpandMethod(
    MethodDeclarationSyntax method,
    string currentClass,
    StringBuilder sb,
    HashSet<string> visited,
    int depth,
    int maxDepth,
    string indent)
{
    if (depth > maxDepth) return;

    IEnumerable<StatementSyntax> body = method.Body?.Statements
        ?? (method.ExpressionBody is not null
            ? (IEnumerable<StatementSyntax>)new[] { SyntaxFactory.ExpressionStatement(method.ExpressionBody.Expression) }
            : Array.Empty<StatementSyntax>());

    foreach (var stmt in body)
    {
        var invocations = stmt.DescendantNodesAndSelf()
            .OfType<InvocationExpressionSyntax>()
            .ToList();

        if (invocations.Count == 0)
        {
            sb.AppendLine(indent + stmt.ToString().TrimEnd());
            continue;
        }

        bool anyExpanded = false;
        foreach (var inv in invocations)
        {
            // メソッド名を取得（this.Foo() / Foo() 両対応）
            string? calleeName = inv.Expression switch
            {
                MemberAccessExpressionSyntax ma => ma.Name.Identifier.Text,
                IdentifierNameSyntax id => id.Identifier.Text,
                _ => null
            };
            if (calleeName is null) continue;

            // 同クラス優先、なければ全体から検索
            var calleeKey = methodMap.ContainsKey($"{currentClass}.{calleeName}")
                ? $"{currentClass}.{calleeName}"
                : methodMap.Keys.FirstOrDefault(k => k.EndsWith($".{calleeName}"));
            if (calleeKey is null) continue;
            if (visited.Contains(calleeKey)) continue;

            var calleeMethod = methodMap[calleeKey];
            anyExpanded = true;
            visited.Add(calleeKey);

            sb.AppendLine($"{indent}// ▼ {calleeKey}() をインライン展開");
            sb.AppendLine($"{indent}{{");

            var parameters = calleeMethod.ParameterList.Parameters;
            if (parameters.Count > 0)
            {
                var paramArgs = parameters.Select((p, i) =>
                {
                    var argText = i < inv.ArgumentList.Arguments.Count
                        ? inv.ArgumentList.Arguments[i].ToString()
                        : p.Default?.Value.ToString() ?? "?";
                    return $"{p.Identifier.Text} = {argText}";
                });
                sb.AppendLine($"{indent}    // 引数: {string.Join(", ", paramArgs)}");
            }

            var calleeClass = (calleeMethod.Parent as ClassDeclarationSyntax)?.Identifier.Text ?? currentClass;
            ExpandMethod(calleeMethod, calleeClass, sb, visited, depth + 1, maxDepth, indent + "    ");

            visited.Remove(calleeKey);
            sb.AppendLine($"{indent}}} // ▲ {calleeName}()");
        }

        if (!anyExpanded)
            sb.AppendLine(indent + stmt.ToString().TrimEnd());
    }
}

bool IsEventHandler(MethodDeclarationSyntax method, Regex namePattern)
{
    if (method.ReturnType.ToString() != "void") return false;

    var name = method.Identifier.Text;
    if (namePattern.IsMatch(name)) return true;

    var parms = method.ParameterList.Parameters;
    if (parms.Count == 2)
    {
        var p0 = parms[0].Type?.ToString() ?? "";
        var p1 = parms[1].Type?.ToString() ?? "";
        if (p0 == "object" && p1.Contains("EventArgs")) return true;
    }

    return false;
}

bool InheritsFromForm(ClassDeclarationSyntax classDecl)
{
    if (classDecl.BaseList is null) return false;
    return classDecl.BaseList.Types.Any(t =>
    {
        var name = t.Type.ToString();
        return name == "Form" || name == "UserControl" ||
               name.EndsWith(".Form") || name.EndsWith(".UserControl");
    });
}

string GetMethodId(MethodDeclarationSyntax method)
{
    var className = (method.Parent as ClassDeclarationSyntax)?.Identifier.Text ?? "";
    return $"{className}.{method.Identifier.Text}";
}

string SanitizeName(string name) =>
    Regex.Replace(name, @"[<>:""/\\|?*]", "_");

string? GetArg(List<string> args, string key)
{
    var idx = args.IndexOf(key);
    return idx >= 0 && idx + 1 < args.Count ? args[idx + 1] : null;
}

string BuildAnalysisPrompt(FormAnalysisResult form)
{
    var sb = new StringBuilder();
    sb.AppendLine("あなたは業務システムのドキュメント作成を支援するAIです。");
    sb.AppendLine("以下は Windows Forms アプリケーションの画面コードです。");
    sb.AppendLine("各イベントハンドラの処理は、呼び出し先の自作メソッドがインライン展開されています。");
    sb.AppendLine("外部ライブラリ（ADO.NET, System.IO, System.Net 等）の呼び出しはそのままです。");
    sb.AppendLine();
    sb.AppendLine("以下の形式でこの画面の機能一覧をMarkdownで出力してください：");
    sb.AppendLine("1. ## 画面概要（2〜3文で画面の目的を説明）");
    sb.AppendLine("2. ## 機能一覧（イベントごとに「機能名」と「処理内容の説明」を箇条書き）");
    sb.AppendLine("3. ## データ操作（DB/ファイル/API等への操作を抽出して列挙）");
    sb.AppendLine("4. ## 注意点・特記事項（エラー処理、権限制御、バリデーション等があれば）");
    sb.AppendLine();
    sb.AppendLine($"# 画面名: {form.FormName}");
    sb.AppendLine();

    foreach (var ev in form.Events)
    {
        sb.AppendLine($"## イベント: {ev.EventName}");
        sb.AppendLine("```csharp");
        var lines = ev.ExpandedCode.Split('\n');
        var truncated = lines.Length > 200;
        sb.AppendLine(string.Join('\n', lines.Take(200)));
        if (truncated) sb.AppendLine("// ... (省略)");
        sb.AppendLine("```");
        sb.AppendLine();
    }

    return sb.ToString();
}

async Task<string> CallClaudeApi(string apiKey, string prompt)
{
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

    var body = new
    {
        model = "claude-sonnet-4-20250514",
        max_tokens = 4096,
        messages = new[] { new { role = "user", content = prompt } }
    };

    var json = JsonSerializer.Serialize(body);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);
    var respBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception($"API Error {response.StatusCode}: {respBody}");

    using var doc = JsonDocument.Parse(respBody);
    return doc.RootElement
        .GetProperty("content")[0]
        .GetProperty("text")
        .GetString() ?? "";
}

// ============================================================
// データクラス
// ============================================================
class FormAnalysisResult
{
    public string FormName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public List<EventExpansion> Events { get; set; } = new();
}

class EventExpansion
{
    public string EventName { get; set; } = "";
    public string ExpandedFile { get; set; } = "";
    public string ExpandedCode { get; set; } = "";
}
