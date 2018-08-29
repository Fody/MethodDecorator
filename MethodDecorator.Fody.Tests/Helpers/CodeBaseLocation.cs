using System;
using System.IO;
using System.Reflection;

public static class CodeBaseLocation
{
    static CodeBaseLocation()
    {
        var assembly = typeof(CodeBaseLocation).Assembly;

        var currentAssemblyPath = assembly.GetAssemblyLocation();
        var currentDirectory = Path.GetDirectoryName(currentAssemblyPath);
        CurrentDirectory = currentDirectory;
    }

    public static string GetAssemblyLocation(this Assembly assembly)
    {
        var uri = new UriBuilder(assembly.CodeBase);
        return Uri.UnescapeDataString(uri.Path);
    }

    public static readonly string CurrentDirectory;
}