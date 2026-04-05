using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodexCodeGuard;

internal enum AssemblyKind
{
    Runtime,
    Editor,
    TestsEditor,
}

internal sealed record Options(
    string RepoRoot,
    string Phase,
    string OwnerThread,
    string Branch,
    IReadOnlyList<string> Paths);

internal sealed record GuardDiagnostic(
    string Severity,
    string RuleId,
    string Message,
    string? FilePath = null,
    int? Line = null,
    int? Column = null,
    string? Assembly = null);

internal sealed class GuardReport
{
    public bool Applies { get; set; }
    public bool CanContinue { get; set; }
    public string Phase { get; set; } = "pre-sync";
    public string RepoRoot { get; set; } = "";
    public string OwnerThread { get; set; } = "";
    public string Branch { get; set; } = "";
    public List<string> ChangedCodeFiles { get; } = [];
    public List<string> AffectedAssemblies { get; } = [];
    public List<string> ChecksRun { get; } = [];
    public List<GuardDiagnostic> Diagnostics { get; } = [];
    public string Summary { get; set; } = "";
    public string Reason { get; set; } = "";
}

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    private static readonly string[] UnityBuiltInDefines =
    [
        "UNITY_6000",
        "UNITY_6000_0",
        "UNITY_6000_0_OR_NEWER",
        "UNITY_STANDALONE",
        "UNITY_STANDALONE_WIN",
        "UNITY_64",
        "UNITY_ASSERTIONS",
        "ENABLE_LEGACY_INPUT_MANAGER",
        "ENABLE_INPUT_SYSTEM"
    ];

    public static int Main(string[] args)
    {
        GuardReport report;
        try
        {
            var options = ParseArgs(args);
            report = Run(options);
        }
        catch (Exception ex)
        {
            report = new GuardReport
            {
                Applies = true,
                CanContinue = false,
                Phase = "pre-sync",
                Summary = "CodexCodeGuard 执行失败",
                Reason = ex.Message
            };
            report.Diagnostics.Add(new GuardDiagnostic("error", "CODEGUARD", ex.ToString()));
        }

        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine(JsonSerializer.Serialize(report, JsonOptions));
        return report.CanContinue ? 0 : 1;
    }

    private static GuardReport Run(Options options)
    {
        var report = new GuardReport
        {
            Applies = false,
            CanContinue = true,
            Phase = options.Phase,
            RepoRoot = options.RepoRoot,
            OwnerThread = options.OwnerThread,
            Branch = options.Branch
        };

        var repoRoot = Path.GetFullPath(options.RepoRoot);
        var changedCodeFiles = NormalizePaths(options.Paths)
            .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .Where(path => IsUnityProjectCodePath(path, repoRoot))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (changedCodeFiles.Count == 0)
        {
            report.Summary = "当前白名单中没有 C# 文件，跳过代码闸门。";
            report.Reason = "无 C# 改动";
            return report;
        }

        report.Applies = true;
        report.ChangedCodeFiles.AddRange(changedCodeFiles);

        var editorRoot = ResolveUnityEditorRoot();
        var assemblyInfos = BuildAssemblyInfos(repoRoot, editorRoot);
        var dirtyState = GitDirtyState.Load(repoRoot);

        var affectedKinds = changedCodeFiles
            .Select(path => ClassifyAssembly(path, repoRoot, assemblyInfos.TestsRoot))
            .Distinct()
            .OrderBy(kind => kind)
            .ToList();

        report.AffectedAssemblies.AddRange(affectedKinds.Select(GetAssemblyName));
        report.ChecksRun.Add("utf8-strict");
        report.ChecksRun.Add("git-diff-check");
        report.ChecksRun.Add("roslyn-assembly-compile");

        RunUtf8Checks(report, changedCodeFiles);
        RunGitDiffCheck(report, repoRoot, changedCodeFiles);

        var compilationResults = CompileAffectedAssemblies(
            repoRoot,
            assemblyInfos,
            dirtyState,
            changedCodeFiles,
            affectedKinds);

        foreach (var result in compilationResults)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                report.Diagnostics.Add(diagnostic);
            }
        }

        report.CanContinue = !report.Diagnostics.Any(item => string.Equals(item.Severity, "error", StringComparison.OrdinalIgnoreCase))
                             && !report.Diagnostics.Any(item => string.Equals(item.Severity, "warning", StringComparison.OrdinalIgnoreCase));

        if (report.CanContinue)
        {
            report.Summary = "代码闸门通过";
            report.Reason = $"已对 {changedCodeFiles.Count} 个 C# 文件完成 UTF-8、diff 和程序集级编译检查。";
        }
        else
        {
            var errors = report.Diagnostics.Count(item => string.Equals(item.Severity, "error", StringComparison.OrdinalIgnoreCase));
            var warnings = report.Diagnostics.Count(item => string.Equals(item.Severity, "warning", StringComparison.OrdinalIgnoreCase));
            report.Summary = "代码闸门未通过";
            report.Reason = $"检测到 {errors} 条错误、{warnings} 条警告；必须先清理后再收口。";
        }

        return report;
    }

    private static void RunUtf8Checks(GuardReport report, IReadOnlyList<string> changedCodeFiles)
    {
        foreach (var filePath in changedCodeFiles)
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            try
            {
                var bytes = File.ReadAllBytes(filePath);
                var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
                var text = utf8.GetString(bytes);

                if (text.Contains('\uFFFD'))
                {
                    report.Diagnostics.Add(new GuardDiagnostic(
                        "error",
                        "ENCODING",
                        "文件包含 Unicode 替换字符 U+FFFD，疑似存在错误解码或坏字符。",
                        filePath));
                }
            }
            catch (DecoderFallbackException)
            {
                report.Diagnostics.Add(new GuardDiagnostic(
                    "error",
                    "ENCODING",
                    "文件不是健康的 UTF-8 文本，禁止继续收口。",
                    filePath));
            }
        }
    }

    private static void RunGitDiffCheck(GuardReport report, string repoRoot, IReadOnlyList<string> changedCodeFiles)
    {
        var args = new List<string> { "-C", repoRoot, "diff", "--check", "--" };
        args.AddRange(changedCodeFiles.Select(path => ToRepoRelativePath(repoRoot, path)));
        var process = RunProcess("git", args, allowFailure: true);
        if (process.ExitCode == 0)
        {
            return;
        }

        foreach (var line in process.Output.Where(item => !string.IsNullOrWhiteSpace(item)))
        {
            report.Diagnostics.Add(new GuardDiagnostic(
                "error",
                "DIFFCHECK",
                line.Trim()));
        }
    }

    private static List<AssemblyCompilationResult> CompileAffectedAssemblies(
        string repoRoot,
        AssemblyDiscovery assemblyInfos,
        GitDirtyState dirtyState,
        IReadOnlyList<string> changedCodeFiles,
        IReadOnlyList<AssemblyKind> affectedKinds)
    {
        var orderedKinds = affectedKinds.OrderBy(kind => kind).ToList();
        var currentResults = CompileSnapshotAssemblies(repoRoot, assemblyInfos, dirtyState, changedCodeFiles, orderedKinds, useAllowedChanges: true);
        var baselineResults = CompileSnapshotAssemblies(repoRoot, assemblyInfos, dirtyState, changedCodeFiles, orderedKinds, useAllowedChanges: false);

        var merged = new List<AssemblyCompilationResult>();
        foreach (var kind in orderedKinds)
        {
            var current = currentResults[kind];
            var baseline = baselineResults[kind];
            var baselineKeys = baseline.Diagnostics
                .Select(BuildDiagnosticKey)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var mergedResult = new AssemblyCompilationResult(current.AssemblyName)
            {
                CompanionReference = current.CompanionReference
            };

            foreach (var diagnostic in current.Diagnostics)
            {
                if (!baselineKeys.Contains(BuildDiagnosticKey(diagnostic)))
                {
                    mergedResult.Diagnostics.Add(diagnostic);
                }
            }

            merged.Add(mergedResult);
        }

        return merged;
    }

    private static Dictionary<AssemblyKind, AssemblyCompilationResult> CompileSnapshotAssemblies(
        string repoRoot,
        AssemblyDiscovery assemblyInfos,
        GitDirtyState dirtyState,
        IReadOnlyList<string> changedCodeFiles,
        IReadOnlyList<AssemblyKind> orderedKinds,
        bool useAllowedChanges)
    {
        var results = new Dictionary<AssemblyKind, AssemblyCompilationResult>();
        MetadataReference? runtimeReference = null;
        MetadataReference? editorReference = null;

        foreach (var kind in orderedKinds)
        {
            var result = CompileAssembly(
                repoRoot,
                assemblyInfos,
                dirtyState,
                changedCodeFiles,
                kind,
                runtimeReference,
                editorReference,
                useAllowedChanges);

            results[kind] = result;

            if (result.CompanionReference is not null)
            {
                if (kind == AssemblyKind.Runtime)
                {
                    runtimeReference = result.CompanionReference;
                }
                else if (kind == AssemblyKind.Editor)
                {
                    editorReference = result.CompanionReference;
                }
            }
        }

        return results;
    }

    private static AssemblyCompilationResult CompileAssembly(
        string repoRoot,
        AssemblyDiscovery assemblyInfos,
        GitDirtyState dirtyState,
        IReadOnlyList<string> changedCodeFiles,
        AssemblyKind kind,
        MetadataReference? runtimeReference,
        MetadataReference? editorReference,
        bool useAllowedChanges)
    {
        var assemblyName = GetAssemblyName(kind);
        var sourceFiles = GetAssemblySourceFiles(repoRoot, assemblyInfos, dirtyState, changedCodeFiles, kind, useAllowedChanges);
        var syntaxTrees = new List<SyntaxTree>();

        foreach (var sourceFile in sourceFiles)
        {
            var text = sourceFile.Content;
            var parseOptions = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.Latest)
                .WithPreprocessorSymbols(GetPreprocessorSymbols(kind));
            syntaxTrees.Add(CSharpSyntaxTree.ParseText(text, parseOptions, sourceFile.VirtualPath));
        }

        var references = BuildMetadataReferences(kind, assemblyInfos, runtimeReference, editorReference);
        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: true,
                warningLevel: 4,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

        var diagnostics = compilation.GetDiagnostics();
        var result = new AssemblyCompilationResult(assemblyName);

        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Severity != DiagnosticSeverity.Error && diagnostic.Severity != DiagnosticSeverity.Warning)
            {
                continue;
            }

            var location = diagnostic.Location;
            string? filePath = null;
            int? line = null;
            int? column = null;
            if (location is { IsInSource: true })
            {
                filePath = location.GetLineSpan().Path;
                var span = location.GetLineSpan().StartLinePosition;
                line = span.Line + 1;
                column = span.Character + 1;
            }

            result.Diagnostics.Add(new GuardDiagnostic(
                diagnostic.Severity == DiagnosticSeverity.Error ? "error" : "warning",
                diagnostic.Id,
                diagnostic.GetMessage(),
                filePath,
                line,
                column,
                assemblyName));
        }

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        if (emitResult.Success)
        {
            stream.Position = 0;
            result.CompanionReference = MetadataReference.CreateFromImage(stream.ToArray());
        }
        else
        {
            foreach (var diagnostic in emitResult.Diagnostics.Where(item => item.Severity == DiagnosticSeverity.Error))
            {
                var location = diagnostic.Location;
                string? filePath = null;
                int? line = null;
                int? column = null;
                if (location is { IsInSource: true })
                {
                    filePath = location.GetLineSpan().Path;
                    var span = location.GetLineSpan().StartLinePosition;
                    line = span.Line + 1;
                    column = span.Character + 1;
                }

                if (!result.Diagnostics.Any(existing =>
                        existing.RuleId == diagnostic.Id &&
                        string.Equals(existing.FilePath, filePath, StringComparison.OrdinalIgnoreCase) &&
                        existing.Line == line &&
                        existing.Column == column))
                {
                    result.Diagnostics.Add(new GuardDiagnostic(
                        "error",
                        diagnostic.Id,
                        diagnostic.GetMessage(),
                        filePath,
                        line,
                        column,
                        assemblyName));
                }
            }
        }

        return result;
    }

    private static List<SourceFileSnapshot> GetAssemblySourceFiles(
        string repoRoot,
        AssemblyDiscovery assemblyInfos,
        GitDirtyState dirtyState,
        IReadOnlyList<string> changedCodeFiles,
        AssemblyKind kind,
        bool useAllowedChanges)
    {
        var trackedSources = RunGit(repoRoot, "ls-files", "--", "*.cs");
        var trackedSet = trackedSources.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidatePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var source in trackedSources)
        {
            var absolute = NormalizePath(Path.Combine(repoRoot, source));
            if (absolute is not null && ClassifyAssembly(absolute, repoRoot, assemblyInfos.TestsRoot) == kind)
            {
                candidatePaths.Add(absolute);
            }
        }

        foreach (var changedFile in changedCodeFiles.Where(path => ClassifyAssembly(path, repoRoot, assemblyInfos.TestsRoot) == kind))
        {
            candidatePaths.Add(changedFile);
        }

        var changedSet = changedCodeFiles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var snapshots = new List<SourceFileSnapshot>();

        foreach (var absolutePath in candidatePaths.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var relativePath = ToRepoRelativePath(repoRoot, absolutePath);
            var normalizedRelative = NormalizePath(Path.Combine(repoRoot, relativePath))!;
            var isTracked = trackedSet.Contains(relativePath);
            var isDirtyTracked = dirtyState.DirtyTracked.Contains(relativePath);
            var isDeletedTracked = dirtyState.DeletedTracked.Contains(relativePath);
            var isUntracked = dirtyState.Untracked.Contains(relativePath);
            var isAllowedChanged = changedSet.Contains(normalizedRelative);

            string? content = null;

            if (useAllowedChanges && isAllowedChanged)
            {
                if (File.Exists(absolutePath))
                {
                    content = ReadWorkingTreeFile(absolutePath);
                }
                else
                {
                    continue;
                }
            }
            else if (isDeletedTracked || (isDirtyTracked && !File.Exists(absolutePath)))
            {
                content = ReadHeadFile(repoRoot, relativePath);
            }
            else if (isDirtyTracked)
            {
                content = ReadHeadFile(repoRoot, relativePath);
            }
            else if (isUntracked && !isAllowedChanged)
            {
                continue;
            }
            else if (File.Exists(absolutePath))
            {
                content = ReadWorkingTreeFile(absolutePath);
            }
            else if (isTracked)
            {
                content = ReadHeadFile(repoRoot, relativePath);
            }

            if (content is not null)
            {
                snapshots.Add(new SourceFileSnapshot(absolutePath, relativePath.Replace('\\', '/'), content));
            }
        }

        return snapshots;
    }

    private static IEnumerable<MetadataReference> BuildMetadataReferences(
        AssemblyKind kind,
        AssemblyDiscovery assemblyInfos,
        MetadataReference? runtimeReference,
        MetadataReference? editorReference)
    {
        var references = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in GetTrustedPlatformAssemblies())
        {
            references[path] = MetadataReference.CreateFromFile(path);
        }

        foreach (var path in assemblyInfos.UnityManagedDlls)
        {
            references[path] = MetadataReference.CreateFromFile(path);
        }

        foreach (var path in assemblyInfos.ScriptAssemblies)
        {
            var fileName = Path.GetFileName(path);
            if (kind == AssemblyKind.Runtime && fileName.Equals("Assembly-CSharp.dll", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (kind == AssemblyKind.Editor && fileName.Equals("Assembly-CSharp-Editor.dll", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (kind == AssemblyKind.TestsEditor && fileName.Equals("Tests.Editor.dll", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            references[path] = MetadataReference.CreateFromFile(path);
        }

        if (kind != AssemblyKind.Runtime)
        {
            if (runtimeReference is not null)
            {
                references["__runtime-temp__"] = runtimeReference;
            }
            else if (assemblyInfos.ProjectAssemblyPaths.TryGetValue("Assembly-CSharp.dll", out var runtimePath))
            {
                references[runtimePath] = MetadataReference.CreateFromFile(runtimePath);
            }
        }

        if (kind == AssemblyKind.TestsEditor)
        {
            foreach (var path in assemblyInfos.ExtraTestDlls)
            {
                references[path] = MetadataReference.CreateFromFile(path);
            }

            if (editorReference is not null)
            {
                references["__editor-temp__"] = editorReference;
            }
            else if (assemblyInfos.ProjectAssemblyPaths.TryGetValue("Assembly-CSharp-Editor.dll", out var editorPath))
            {
                references[editorPath] = MetadataReference.CreateFromFile(editorPath);
            }
        }

        return references.Values;
    }

    private static string[] GetPreprocessorSymbols(AssemblyKind kind)
    {
        return kind switch
        {
            AssemblyKind.Runtime => UnityBuiltInDefines,
            AssemblyKind.Editor => UnityBuiltInDefines.Concat(["UNITY_EDITOR"]).ToArray(),
            AssemblyKind.TestsEditor => UnityBuiltInDefines.Concat(["UNITY_EDITOR", "UNITY_INCLUDE_TESTS"]).ToArray(),
            _ => UnityBuiltInDefines
        };
    }

    private static bool IsUnityProjectCodePath(string absolutePath, string repoRoot)
    {
        var relative = ToRepoRelativePath(repoRoot, absolutePath);
        return relative.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) ||
               relative.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase);
    }

    private static AssemblyKind ClassifyAssembly(string absolutePath, string repoRoot, string testsRoot)
    {
        var normalized = NormalizePath(absolutePath)!;
        if (normalized.StartsWith(testsRoot, StringComparison.OrdinalIgnoreCase))
        {
            return AssemblyKind.TestsEditor;
        }

        var relative = ToRepoRelativePath(repoRoot, normalized);
        if (relative.Contains("/Editor/", StringComparison.OrdinalIgnoreCase) ||
            relative.StartsWith("Assets/Editor/", StringComparison.OrdinalIgnoreCase))
        {
            return AssemblyKind.Editor;
        }

        return AssemblyKind.Runtime;
    }

    private static string GetAssemblyName(AssemblyKind kind)
    {
        return kind switch
        {
            AssemblyKind.Runtime => "Assembly-CSharp",
            AssemblyKind.Editor => "Assembly-CSharp-Editor",
            AssemblyKind.TestsEditor => "Tests.Editor",
            _ => kind.ToString()
        };
    }

    private static string ResolveUnityEditorRoot()
    {
        var editorLog = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Unity",
            "Editor",
            "Editor.log");

        if (!File.Exists(editorLog))
        {
            throw new InvalidOperationException("未找到 Unity Editor.log，无法解析 Unity Editor 安装路径。");
        }

        using var stream = new FileStream(editorLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (!line.EndsWith(@"\Editor\Unity.exe", StringComparison.OrdinalIgnoreCase) &&
                !line.EndsWith(@"/Editor/Unity.exe", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var trimmed = line.Trim();
            if (File.Exists(trimmed))
            {
                return Path.GetDirectoryName(trimmed)!;
            }
        }

        throw new InvalidOperationException("Editor.log 中未找到可用的 Unity.exe 路径。");
    }

    private static AssemblyDiscovery BuildAssemblyInfos(string repoRoot, string editorRoot)
    {
        var managedRoot = Path.Combine(editorRoot, "Data", "Managed");
        if (!Directory.Exists(managedRoot))
        {
            throw new InvalidOperationException($"Unity Managed 目录不存在：{managedRoot}");
        }

        var unityDlls = Directory.GetFiles(managedRoot, "*.dll", SearchOption.AllDirectories)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var scriptAssemblyRoot = Path.Combine(repoRoot, "Library", "ScriptAssemblies");
        if (!Directory.Exists(scriptAssemblyRoot))
        {
            throw new InvalidOperationException($"ScriptAssemblies 目录不存在：{scriptAssemblyRoot}");
        }

        var scriptAssemblyDlls = Directory.GetFiles(scriptAssemblyRoot, "*.dll", SearchOption.TopDirectoryOnly)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var map = scriptAssemblyDlls.ToDictionary(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
        var testsRoot = NormalizePath(Path.Combine(repoRoot, "Assets", "YYY_Tests", "Editor"))
            ?? throw new InvalidOperationException("无法确定 Tests.Editor 根目录。");
        var extraTestDlls = Directory.GetFiles(Path.Combine(editorRoot, "Data"), "*.dll", SearchOption.AllDirectories)
            .Where(path => Path.GetFileName(path).Equals("nunit.framework.dll", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new AssemblyDiscovery(unityDlls, scriptAssemblyDlls, map, testsRoot, extraTestDlls);
    }

    private static IEnumerable<string> GetTrustedPlatformAssemblies()
    {
        var value = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string ReadHeadFile(string repoRoot, string relativePath)
    {
        var output = RunGitRaw(repoRoot, "show", $"HEAD:{relativePath.Replace('\\', '/')}");
        if (!output.Success)
        {
            throw new InvalidOperationException($"无法从 HEAD 读取基线文件：{relativePath}");
        }

        return output.StandardOutput;
    }

    private static Options ParseArgs(string[] args)
    {
        string? repoRoot = null;
        string phase = "pre-sync";
        string ownerThread = "";
        string branch = "";
        var paths = new List<string>();

        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--repo-root":
                    repoRoot = args[++index];
                    break;
                case "--phase":
                    phase = args[++index];
                    break;
                case "--owner-thread":
                    ownerThread = args[++index];
                    break;
                case "--branch":
                    branch = args[++index];
                    break;
                case "--path":
                    paths.Add(args[++index]);
                    break;
                default:
                    throw new ArgumentException($"未知参数：{args[index]}");
            }
        }

        if (string.IsNullOrWhiteSpace(repoRoot))
        {
            throw new ArgumentException("必须提供 --repo-root。");
        }

        return new Options(
            Path.GetFullPath(repoRoot),
            phase,
            ownerThread,
            branch,
            NormalizePaths(paths));
    }

    private static List<string> NormalizePaths(IEnumerable<string> paths)
    {
        return paths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => NormalizePath(path)!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? NormalizePath(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return Path.GetFullPath(value);
        }
        catch
        {
            return null;
        }
    }

    private static string ToRepoRelativePath(string repoRoot, string absolutePath)
    {
        return Path.GetRelativePath(repoRoot, absolutePath).Replace('\\', '/');
    }

    private static ProcessResult RunProcess(string fileName, IEnumerable<string> arguments, bool allowFailure = false)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = System.Diagnostics.Process.Start(startInfo)
            ?? throw new InvalidOperationException($"无法启动进程：{fileName}");
        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!allowFailure && process.ExitCode != 0)
        {
            throw new InvalidOperationException($"{fileName} 失败：{standardError}{standardOutput}");
        }

        var output = new List<string>();
        if (!string.IsNullOrWhiteSpace(standardOutput))
        {
            output.AddRange(standardOutput.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries));
        }
        if (!string.IsNullOrWhiteSpace(standardError))
        {
            output.AddRange(standardError.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries));
        }

        return new ProcessResult(process.ExitCode == 0, process.ExitCode, standardOutput, standardError, output);
    }

    private static string ReadWorkingTreeFile(string absolutePath)
    {
        using var stream = File.OpenRead(absolutePath);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    private static ProcessResult RunGitRaw(string repoRoot, params string[] arguments)
    {
        var args = new List<string> { "-C", repoRoot };
        args.AddRange(arguments);
        return RunProcess("git", args, allowFailure: true);
    }

    internal static List<string> RunGit(string repoRoot, params string[] arguments)
    {
        var result = RunGitRaw(repoRoot, arguments);
        if (!result.Success)
        {
            throw new InvalidOperationException($"git {string.Join(" ", arguments)} 失败：{result.StandardError}{result.StandardOutput}");
        }

        return result.StandardOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    private static string BuildDiagnosticKey(GuardDiagnostic diagnostic)
    {
        return string.Join("|",
            diagnostic.Severity,
            diagnostic.RuleId,
            diagnostic.Assembly ?? string.Empty,
            diagnostic.FilePath ?? string.Empty,
            diagnostic.Line?.ToString() ?? string.Empty,
            diagnostic.Column?.ToString() ?? string.Empty,
            diagnostic.Message);
    }
}

internal sealed record AssemblyDiscovery(
    IReadOnlyList<string> UnityManagedDlls,
    IReadOnlyList<string> ScriptAssemblies,
    IReadOnlyDictionary<string, string> ProjectAssemblyPaths,
    string TestsRoot,
    IReadOnlyList<string> ExtraTestDlls);

internal sealed record SourceFileSnapshot(string AbsolutePath, string VirtualPath, string Content);

internal sealed class AssemblyCompilationResult(string assemblyName)
{
    public string AssemblyName { get; } = assemblyName;
    public List<GuardDiagnostic> Diagnostics { get; } = [];
    public MetadataReference? CompanionReference { get; set; }
}

internal sealed class GitDirtyState
{
    public HashSet<string> DirtyTracked { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> DeletedTracked { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Untracked { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static GitDirtyState Load(string repoRoot)
    {
        var state = new GitDirtyState();
        foreach (var line in Program.RunGit(repoRoot, "diff", "--name-status", "HEAD", "--"))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                continue;
            }

            var status = parts[0].Trim();
            var path = parts[^1].Trim().Replace('\\', '/');
            state.DirtyTracked.Add(path);
            if (status.StartsWith("D", StringComparison.OrdinalIgnoreCase))
            {
                state.DeletedTracked.Add(path);
            }
        }

        foreach (var line in Program.RunGit(repoRoot, "ls-files", "--others", "--exclude-standard"))
        {
            var path = line.Trim().Replace('\\', '/');
            if (!string.IsNullOrWhiteSpace(path))
            {
                state.Untracked.Add(path);
            }
        }

        return state;
    }
}

internal sealed record ProcessResult(
    bool Success,
    int ExitCode,
    string StandardOutput,
    string StandardError,
    IReadOnlyList<string> Output);
