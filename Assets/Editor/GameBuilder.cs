using System.Linq;
using UnityEditor;
using UnityEngine;

// Batch-mode build entry points for Florestia.
// Invoked from CLI, e.g.:
//   Unity -quit -batchmode -projectPath . -executeMethod GameBuilder.BuildMacOS
public static class GameBuilder
{
    static string[] Scenes =>
        EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

    [MenuItem("Florestia/Build/macOS (Apple Silicon)")]
    public static void BuildMacOS()
    {
        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = "Builds/macOS/Florestia.app",
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };
        Run(options);
    }

    [MenuItem("Florestia/Build/Windows (x64)")]
    public static void BuildWindows()
    {
        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = "Builds/Windows/Florestia.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        Run(options);
    }

    static void Run(BuildPlayerOptions options)
    {
        Debug.Log($"[GameBuilder] Building {options.target} with {options.scenes.Length} scene(s) -> {options.locationPathName}");
        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        Debug.Log($"[GameBuilder] Result: {summary.result}, size: {summary.totalSize} bytes, errors: {summary.totalErrors}");

        if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
