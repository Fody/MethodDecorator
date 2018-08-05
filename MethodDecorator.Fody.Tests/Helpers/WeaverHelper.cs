﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MethodDecorator.Fody.Interfaces;
using Mono.Cecil;

public class WeaverHelper
{
    string projectPath;
    string assemblyPath;

    public WeaverHelper(string projectPath)
    {
        this.projectPath =
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\TestAssemblies", projectPath));
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    public Assembly Weave()
    {
        GetAssemblyPath();

        var newAssembly = assemblyPath.Replace(".dll", "2.dll");

        var assemblyFileName = Path.GetFileName(newAssembly);
        var assembly =
            AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic)
                .FirstOrDefault(a => Path.GetFileName(a.CodeBase) == assemblyFileName);
        if (assembly != null)
            return assembly;

        File.Copy(assemblyPath.Replace(".dll", ".pdb"), newAssembly.Replace(".dll", ".pdb"), true);

        var assemblyResolver = new TestAssemblyResolver(assemblyPath, projectPath);

        using (var assemblyFileStream = File.Open(
            assemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var moduleDefinition = ModuleDefinition.ReadModule(
                assemblyFileStream,
                new ReaderParameters
                {
                    AssemblyResolver = assemblyResolver,
                    ReadSymbols = true
                }))
            {
                var weavingTask = new ModuleWeaver
                {
                    ModuleDefinition = moduleDefinition,
                    AssemblyResolver = assemblyResolver
                };

                weavingTask.Execute();

                moduleDefinition.Write(
                    newAssembly,
                    new WriterParameters
                    {
                        WriteSymbols = true
                    });
            }
        }

        PEVerify(newAssembly);

        return Assembly.LoadFile(newAssembly);
    }

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        if (args.Name.Contains("AnotherAssemblyAttributeContainer"))
        {
            var path = Path.Combine(
                Path.GetDirectoryName(projectPath),
                GetOutputPathValue(),
                "AnotherAssemblyAttributeContainer.dll");
            return Assembly.LoadFile(path);
        }

        if (args.Name.Contains("MethodDecoratorInterfaces"))
        {
            return typeof(IMethodDecorator).Assembly;
        }

        return null;
    }

    void GetAssemblyPath()
    {
        assemblyPath = Path.Combine(
            Path.GetDirectoryName(projectPath),
            GetOutputPathValue(),
            GetAssemblyName() + ".dll");
    }

    string GetAssemblyName()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        return xDocument.Descendants("AssemblyName")
            .Select(x => x.Value)
            .First();
    }

    string GetOutputPathValue()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        var outputPathValue = (from propertyGroup in xDocument.Descendants("PropertyGroup")
            let condition = ((string) propertyGroup.Attribute("Condition"))
            where (condition != null) &&
                  (condition.Trim() == "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
            from outputPath in propertyGroup.Descendants("OutputPath")
            select outputPath.Value).First();
#if (!DEBUG)
        outputPathValue = outputPathValue.Replace("Debug", "Release");
#endif
        return outputPathValue;
    }

    void PEVerify(string assemblyLocation)
    {
        var pathKeys = new[]
        {
            "sdkDir",
            "x86SdkDir",
            "sdkDirUnderVista"
        };

        var process = new Process();
        var peVerifyLocation = string.Empty;

        peVerifyLocation = GetPEVerifyLocation(pathKeys, peVerifyLocation);

        if (!File.Exists(peVerifyLocation))
        {
            Console.WriteLine("Warning: PEVerify.exe could not be found. Skipping test.");

            return;
        }

        process.StartInfo.FileName = peVerifyLocation;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        process.StartInfo.Arguments = "\"" + assemblyLocation + "\" /VERBOSE /NOLOGO";
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        var processOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var result = $"PEVerify Exit Code: {process.ExitCode}";

        Console.WriteLine(GetType().FullName + ": " + result);

        if (process.ExitCode == 0)
        {
            return;
        }

        Console.WriteLine(processOutput);
        throw new Exception($"{result}{Environment.NewLine}PEVerify output: {Environment.NewLine}{processOutput}");
    }

    static string GetPEVerifyLocation(IEnumerable<string> pathKeys, string peVerifyLocation)
    {
        foreach (var key in pathKeys)
        {
            var directory = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(directory))
                continue;

            peVerifyLocation = Path.Combine(directory, "peverify.exe");

            if (File.Exists(peVerifyLocation))
                break;
        }

        return peVerifyLocation;
    }
}