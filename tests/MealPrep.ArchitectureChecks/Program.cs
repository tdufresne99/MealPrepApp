using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Xml.Linq;

var root = Path.GetFullPath(args.Length == 0 ? "." : args[0]);
var modules = new[] { "Identity", "Households", "Recipes", "MealPlanning", "Groceries" };
var errors = new List<string>();
var projects = new Dictionary<string, ProjectBoundary>(StringComparer.OrdinalIgnoreCase);

try
{
    foreach (var file in Directory.GetFiles(Path.Combine(root, "src"), "*.csproj", SearchOption.AllDirectories))
    {
        var name = Path.GetFileNameWithoutExtension(file);
        ProjectBoundary boundary;
        if (name == "MealPrep.Api")
        {
            boundary = new(name, null, "Api", file);
        }
        else
        {
            var parts = name.Split('.');
            if (parts.Length != 3 || parts[0] != "MealPrep" || !modules.Contains(parts[1]) ||
                !new[] { "Domain", "Application", "Infrastructure", "Contracts" }.Contains(parts[2]))
            {
                errors.Add($"Unclassified source project: {file}. Define its boundary before adding it.");
                continue;
            }
            boundary = new(name, parts[1], parts[2], file);
        }

        var expected = boundary.Layer == "Api"
            ? Path.Combine(root, "src", name, name + ".csproj")
            : Path.Combine(root, "src", "Modules", boundary.Module!, name, name + ".csproj");
        if (!Path.GetFullPath(file).Equals(expected, StringComparison.OrdinalIgnoreCase))
            errors.Add($"{name}: project must be located at {expected}.");
        projects.Add(Path.GetFullPath(file), boundary);
    }

    foreach (var module in modules)
        foreach (var layer in new[] { "Domain", "Application", "Infrastructure" })
            if (!projects.Values.Any(p => p.Name == $"MealPrep.{module}.{layer}"))
                errors.Add($"Missing required project: MealPrep.{module}.{layer}.");
    if (!projects.Values.Any(p => p.Layer == "Api"))
        errors.Add("Missing API host.");

    var solution = XDocument.Load(Path.Combine(root, "MealPrepApp.slnx"));
    var solutionProjects = solution.Descendants("Project")
        .Select(p => Path.GetFullPath(Path.Combine(root, p.Attribute("Path")!.Value)))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    foreach (var project in projects.Values)
        if (!solutionProjects.Contains(project.File))
            errors.Add($"{project.Name}: source project must be included in the solution.");

    foreach (var project in projects.Values.OrderBy(p => p.Name))
    {
        foreach (var configuration in new[] { "Debug", "Release" })
        {
            using var evaluation = await Evaluate(project.File, configuration, root);
            var properties = evaluation.RootElement.GetProperty("Properties");
            var items = evaluation.RootElement.GetProperty("Items");
            var context = $"{project.Name} [{configuration}]";
            if (properties.GetProperty("TargetFramework").GetString() != "net10.0" ||
                !string.IsNullOrEmpty(properties.GetProperty("TargetFrameworks").GetString()))
                errors.Add($"{context}: expected a single net10.0 target; update the architecture checks for a new target.");
            if (properties.GetProperty("AssemblyName").GetString() != project.Name)
                errors.Add($"{context}: assembly name must match its project boundary.");

            foreach (var reference in items.GetProperty("ProjectReference").EnumerateArray())
            {
                var path = Path.GetFullPath(reference.GetProperty("FullPath").GetString()!);
                if (!projects.TryGetValue(path, out var target) || !CanReference(project, target))
                    errors.Add($"{context}: forbidden project reference to {Path.GetFileNameWithoutExtension(path)}.");
            }

            // Explicit DLL references could bypass the checked project/package graph.
            foreach (var reference in items.GetProperty("Reference").EnumerateArray())
                errors.Add($"{context}: explicit DLL reference {reference.GetProperty("Identity").GetString()} is not allowed; use a reviewed project or package reference.");

            if (project.Layer is "Domain" or "Application" or "Contracts")
            {
                foreach (var package in items.GetProperty("PackageReference").EnumerateArray())
                    errors.Add($"{context}: package {package.GetProperty("Identity").GetString()} is not allowed in the inner layers; approve any exception explicitly.");
                foreach (var framework in items.GetProperty("FrameworkReference").EnumerateArray())
                    if (framework.GetProperty("Identity").GetString() != "Microsoft.NETCore.App")
                        errors.Add($"{context}: framework {framework.GetProperty("Identity").GetString()} is not allowed in the inner layers.");
            }

            if (configuration == "Release")
                CheckAssembly(project, properties.GetProperty("TargetPath").GetString()!);
        }
    }
}
catch (Exception exception)
{
    errors.Add($"Architecture verification could not complete: {exception.Message}");
}

foreach (var error in errors)
    Console.Error.WriteLine(error);
if (errors.Count > 0)
{
    Console.Error.WriteLine($"Architecture checks failed: {errors.Count} violation(s).");
    return 1;
}
Console.WriteLine($"Architecture checks passed for {projects.Count} source projects (Debug/Release references and Release assemblies).");
return 0;

bool CanReference(ProjectBoundary source, ProjectBoundary target) => source.Layer switch
{
    "Domain" or "Contracts" => false,
    "Application" => source.Module == target.Module && target.Layer is "Domain" or "Contracts",
    "Infrastructure" => (source.Module == target.Module && target.Layer is "Domain" or "Application" or "Contracts")
        || (source.Module != target.Module && target.Layer == "Contracts"),
    "Api" => target.Layer is "Application" or "Infrastructure",
    _ => false
};

void CheckAssembly(ProjectBoundary project, string path)
{
    if (!File.Exists(path))
    {
        errors.Add($"{project.Name}: missing Release assembly. Build the solution before running architecture checks.");
        return;
    }
    using var stream = File.OpenRead(path);
    using var reader = new PEReader(stream);
    var metadata = reader.GetMetadataReader();
    foreach (var handle in metadata.AssemblyReferences)
    {
        var name = metadata.GetString(metadata.GetAssemblyReference(handle).Name);
        if (name.StartsWith("MealPrep.", StringComparison.Ordinal))
        {
            var target = projects.Values.SingleOrDefault(p => p.Name == name);
            if (target is null || !CanReference(project, target))
                errors.Add($"{project.Name}: forbidden compiled assembly reference to {name}.");
        }
        if (project.Layer is "Domain" or "Application" or "Contracts")
        {
            // BCL HTTP/database APIs do not require a NuGet dependency, so also inspect emitted references.
            if (name.StartsWith("System.Net", StringComparison.Ordinal) ||
                name.StartsWith("System.Data", StringComparison.Ordinal) ||
                name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) ||
                name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal) ||
                name.StartsWith("Microsoft.Extensions", StringComparison.Ordinal) ||
                name.StartsWith("Npgsql", StringComparison.Ordinal))
                errors.Add($"{project.Name}: forbidden compiled framework reference to {name}.");
        }
    }
}

static async Task<JsonDocument> Evaluate(string project, string configuration, string root)
{
    var startInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
    {
        WorkingDirectory = root,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    foreach (var argument in new[] { "msbuild", project, "-nologo",
        "-getProperty:TargetFramework,TargetFrameworks,TargetPath,AssemblyName",
        "-getItem:ProjectReference,PackageReference,FrameworkReference,Reference",
        $"-property:Configuration={configuration}" })
        startInfo.ArgumentList.Add(argument);
    using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Cannot start MSBuild.");
    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();
    var output = await outputTask;
    var error = await errorTask;
    if (process.ExitCode != 0)
        throw new InvalidOperationException($"MSBuild evaluation failed for {project}: {output} {error}");
    return JsonDocument.Parse(output);
}

internal sealed record ProjectBoundary(string Name, string? Module, string Layer, string File);
