using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Mono.Cecil;

public class WeaverHelper
{
    private readonly string projectPath;
    private string assemblyPath;

    public WeaverHelper(string projectPath)
    {
        this.projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\TestAssemblies", projectPath));
    }

    public Assembly Weave()
    {
        GetAssemblyPath();

        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssembly, true);
        File.Copy(assemblyPath.Replace(".dll", ".pdb"), newAssembly.Replace(".dll", ".pdb"), true);


        var assemblyResolver = new TestAssemblyResolver(assemblyPath, projectPath);
        var moduleDefinition = ModuleDefinition.ReadModule(newAssembly, new ReaderParameters
                                                                        {
                                                                            AssemblyResolver = assemblyResolver,
                                                                            ReadSymbols = true
                                                                        });
        var weavingTask = new ModuleWeaver
                          {
                              ModuleDefinition = moduleDefinition,
                              AssemblyResolver = assemblyResolver
                          };

        weavingTask.Execute();

        moduleDefinition.Write(newAssembly, new WriterParameters
                                            {
                                                WriteSymbols = true
                                            });

        PEVerify(newAssembly);

        return Assembly.LoadFile(newAssembly);
    }

    private void GetAssemblyPath()
    {
        assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), GetOutputPathValue(), GetAssemblyName() + ".dll");
    }

    private string GetAssemblyName()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        return xDocument.Descendants("AssemblyName")
            .Select(x => x.Value)
            .First();
    }

    private string GetOutputPathValue()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        var outputPathValue = (from propertyGroup in xDocument.Descendants("PropertyGroup")
                               let condition = ((string)propertyGroup.Attribute("Condition"))
                               where (condition != null) &&
                                     (condition.Trim() == "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
                               from outputPath in propertyGroup.Descendants("OutputPath")
                               select outputPath.Value).First();
#if (!DEBUG)
            outputPathValue = outputPathValue.Replace("Debug", "Release");
#endif
        return outputPathValue;
    }

    private void PEVerify(string assemblyLocation)
    {
        var pathKeys = new[]
                               {
                                   "sdkDir",
                                   "x86SdkDir",
                                   "sdkDirUnderVista"
                               };

        var process = new Process();
        string peVerifyLocation = string.Empty;


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

        string processOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        string result = string.Format("PEVerify Exit Code: {0}", process.ExitCode);

        Console.WriteLine(GetType().FullName + ": " + result);

        if (process.ExitCode == 0)
            return;

        Console.WriteLine(processOutput);
        throw new Exception(result + Environment.NewLine + "PEVerify output: " + Environment.NewLine + processOutput);
    }

    private static string GetPEVerifyLocation(IEnumerable<string> pathKeys, string peVerifyLocation)
    {
        foreach (string key in pathKeys)
        {
            string directory = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(directory))
                continue;

            peVerifyLocation = Path.Combine(directory, "peverify.exe");

            if (File.Exists(peVerifyLocation))
                break;
        }
        return peVerifyLocation;
    }

}