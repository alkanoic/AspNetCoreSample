// InlineExpander
// 使い方:
//   dotnet run -- --solution "C:\path\to\MyApp.sln" --apikey "sk-ant-..."

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

// ============================================================
// 設定
// ============================================================
var argList = args.ToList();
string solutionPath = GetArg(argList, "--solution") ?? throw new Exception("--solution を指定してください");
string apiKey       = GetArg(argList, "--apikey")   ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
string outputDir    = GetArg(argList, "--output")   ?? Path.Combine(Path.GetDirectoryName(Path.GetFullPath(solutionPath))!, "form_analysis");
int    maxDepth     = int.Parse(GetArg(argList, "--depth") ?? "10");
bool   skipApi      = string.IsNullOrEmpty(apiKey);

Directory.CreateDirectory(Path.Combine(outputDir, "expanded"));
Directory.CreateDirectory(Path.Combine(outputDir, "features"));

Console.WriteLine($"[設定] ソリューション : {solutionPath}");
Console.WriteLine($"[設定] 出力先         : {outputDir}");
Console.WriteLine($"[設定] API呼び出し    : {(skipApi ? "スキップ（--apikey 未指定）" : "有効")}");
Console.WriteLine();

// ============================================================
// MSBuildWorkspace 初期化
// ============================================================
MSBuildLocator.RegisterDefaults();
using var workspace = MSBuildWorkspace.Create();
workspace.WorkspaceFailed += (_, e) => Console.WriteLine($"[警告] {e.Diagnostic.Message}");

Console.WriteLine("[1/4] ソリューションを読み込み中...");
var solution = await workspace.OpenSolutionAsync(solutionPath);

// ============================================================
// 全プロジェクトのメソッド定義をキャッシュ
// ============================================================
Console.WriteLine("[2/4] プロジェクト全体を解析中...");

var methodMap            = new Dictionary<string, (SemanticModel model, MethodDeclarationSyntax syntax)>();
var compilationByProject = new Dictionary<string, Compilation>();

foreach (var project in solution.Projects)
{
    var compilation = await project.GetCompilationAsync();
    if (compilation is null) continue;
    compilationByProject[project.Name] = compilation;

    foreach (var tree in compilation.SyntaxTrees)
    {
        var model = compilation.GetSemanticModel(tree);
        var root  = await tree.GetRootAsync();
        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            var sym = model.GetDeclaredSymbol(method);
            if (sym is null) continue;
            var id = sym.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (!methodMap.ContainsKey(id))
                methodMap[id] = (model, method);
        }
    }
}
Console.WriteLine($"       {methodMap.Count} メソッドをキャッシュ");

// ============================================================
// 各 Form のイベントハンドラを収集して展開
// ============================================================
Console.WriteLine("[3/4] Formイベントハンドラを展開中...");

var eventHandlerPattern = new Regex(@"_(Click|Load|Changed|Enter|Leave|KeyDown|KeyUp|KeyPress|DoubleClick|MouseDown|MouseUp|FormClosing|FormClosed|Shown|Resize|SelectedIndexChanged|TextChanged|CheckedChanged|ValueChanged|CellClick|CellValueChanged|RowEnter|RowLeave|SelectionChanged|AfterSelect|NodeMouseClick|Paint|Validated|Validating|DropDown|DropDownClosed|ItemActivate|ColumnClick|DrawItem|MeasureItem|Scroll|Tick)$");

var formResults = new List<FormAnalysisResult>();

foreach (var project in solution.Projects)
{
    if (!compilationByProject.TryGetValue(project.Name, out var compilation)) continue;

    foreach (var tree in compilation.SyntaxTrees)
    {
        var filePath = tree.FilePath;
        if (filePath.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)) continue;

        var model = compilation.GetSemanticModel(tree);
        var root  = await tree.GetRootAsync();

        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            var classSym = model.GetDeclaredSymbol(classDecl);
            if (classSym is null) continue;
            if (!InheritsFromForm(classSym)) continue;

            var formName   = classSym.Name;
            Console.WriteLine($"       Form発見: {formName}");

            var formResult = new FormAnalysisResult { FormName = formName, FilePath = filePath };

            foreach (var method in classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                var sym = model.GetDeclaredSymbol(method);
                if (sym is null) continue;
                if (!IsEventHandler(sym, eventHandlerPattern)) continue;

                Console.WriteLine($"         イベント: {sym.Name}");

                var sb      = new StringBuilder();
                var visited = new HashSet<string>();
                sb.AppendLine($"// === {formName}.{sym.Name} ===");
                sb.AppendLine($"// ファイル: {filePath}");
                sb.AppendLine();
                ExpandMethod(method, model, project.AssemblyName, sb, visited, 0, maxDepth, "");

                var expandedCode  = sb.ToString();
                var safeFormName  = SanitizeName(formName);
                var safeEventName = SanitizeName(sym.Name);
                var expandedDir   = Path.Combine(outputDir, "expanded", safeFormName);
                Directory.CreateDirectory(expandedDir);
                var expandedFile  = Path.Combine(expandedDir, $"{safeEventName}.txt");
                await File.WriteAllTextAsync(expandedFile, expandedCode, Encoding.UTF8);

                formResult.Events.Add(new EventExpansion
                {
                    EventName    = sym.Name,
                    ExpandedFile = expandedFile,
                    ExpandedCode = expandedCode
                });
            }

            if (formResult.Events.Count > 0)
                formResults.Add(formResult);
        }
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
    SemanticModel model,
    string projectAssembly,
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
            if (model.GetSymbolInfo(inv).Symbol is not IMethodSymbol callee) continue;
            if (!IsUserDefinedMethod(callee, projectAssembly)) continue;

            var calleeId = callee.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (visited.Contains(calleeId)) continue;
            if (!methodMap.TryGetValue(calleeId, out var calleeEntry)) continue;

            anyExpanded = true;
            visited.Add(calleeId);

            sb.AppendLine($"{indent}// ▼ {callee.ContainingType.Name}.{callee.Name}() をインライン展開");
            sb.AppendLine($"{indent}{{");

            if (callee.Parameters.Length > 0)
            {
                var paramArgs = callee.Parameters.Select((p, i) =>
                {
                    var argText = i < inv.ArgumentList.Arguments.Count
                        ? inv.ArgumentList.Arguments[i].ToString()
                        : p.HasExplicitDefaultValue ? p.ExplicitDefaultValue?.ToString() ?? "null" : "?";
                    return $"{p.Name} = {argText}";
                });
                sb.AppendLine($"{indent}    // 引数: {string.Join(", ", paramArgs)}");
            }

            ExpandMethod(calleeEntry.syntax, calleeEntry.model, projectAssembly,
                sb, visited, depth + 1, maxDepth, indent + "    ");

            visited.Remove(calleeId);
            sb.AppendLine($"{indent}}} // ▲ {callee.Name}()");
        }

        if (!anyExpanded)
            sb.AppendLine(indent + stmt.ToString().TrimEnd());
    }
}

bool IsEventHandler(IMethodSymbol sym, Regex namePattern)
{
    if (!sym.ReturnsVoid) return false;
    if (namePattern.IsMatch(sym.Name)) return true;
    if (sym.Parameters.Length == 2)
    {
        var p0 = sym.Parameters[0].Type.ToDisplayString();
        var p1 = sym.Parameters[1].Type.ToDisplayString();
        if (p0 == "object" && p1.Contains("EventArgs")) return true;
    }
    return false;
}

bool InheritsFromForm(INamedTypeSymbol sym)
{
    var current = sym.BaseType;
    while (current is not null)
    {
        var name = current.ToDisplayString();
        if (name is "System.Windows.Forms.Form" or "System.Windows.Forms.UserControl")
            return true;
        current = current.BaseType;
    }
    return false;
}

bool IsUserDefinedMethod(IMethodSymbol sym, string projectAssembly)
{
    var asm = sym.ContainingAssembly?.Name ?? "";
    return asm == projectAssembly || compilationByProject.ContainsKey(asm);
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
        var lines     = ev.ExpandedCode.Split('\n');
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
        model      = "claude-sonnet-4-20250514",
        max_tokens = 4096,
        messages   = new[] { new { role = "user", content = prompt } }
    };

    var json     = JsonSerializer.Serialize(body);
    var content  = new StringContent(json, Encoding.UTF8, "application/json");
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
record FormAnalysisResult
{
    public string FormName { get; init; } = "";
    public string FilePath { get; init; } = "";
    public List<EventExpansion> Events { get; init; } = new();
}

record EventExpansion
{
    public string EventName    { get; init; } = "";
    public string ExpandedFile { get; init; } = "";
    public string ExpandedCode { get; init; } = "";
}
