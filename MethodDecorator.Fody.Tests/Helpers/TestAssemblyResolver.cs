using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Mono.Cecil;

public class TestAssemblyResolver : IAssemblyResolver {
    private readonly List<string> directories;
    private List<string> gacPaths;

    public TestAssemblyResolver(string targetPath, string projectPath) {
        var versionReader = new VersionReader(projectPath);
        this.directories = new List<string>();

        if (versionReader.IsSilverlight) {
            if (string.IsNullOrEmpty(versionReader.TargetFrameworkProfile)) {
                this.directories.Add(
                    string.Format(
                        @"{0}\Reference Assemblies\Microsoft\Framework\Silverlight\{1}\",
                        this.GetProgramFilesPath(),
                        versionReader.FrameworkVersionAsString));
            }
            else {
                this.directories.Add(
                    string.Format(
                        @"{0}\Reference Assemblies\Microsoft\Framework\Silverlight\{1}\Profile\{2}",
                        this.GetProgramFilesPath(),
                        versionReader.FrameworkVersionAsString,
                        versionReader.TargetFrameworkProfile));
            }
        }
        else {
            if (string.IsNullOrEmpty(versionReader.TargetFrameworkProfile)) {
                if (versionReader.FrameworkVersionAsNumber == 3.5m) {
                    this.directories.Add(
                        string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\v3.5\", this.GetProgramFilesPath()));
                    this.directories.Add(
                        string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\v3.0\", this.GetProgramFilesPath()));
                    this.directories.Add(
                        Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework\v2.0.50727\"));
                }
                else {
                    this.directories.Add(
                        string.Format(
                            @"{0}\Reference Assemblies\Microsoft\Framework\.NETFramework\{1}\",
                            this.GetProgramFilesPath(),
                            versionReader.FrameworkVersionAsString));
                }
            }
            else {
                this.directories.Add(
                    string.Format(
                        @"{0}\Reference Assemblies\Microsoft\Framework\.NETFramework\{1}\Profile\{2}",
                        this.GetProgramFilesPath(),
                        versionReader.FrameworkVersionAsString,
                        versionReader.TargetFrameworkProfile));
            }
        }
        this.directories.Add(Path.GetDirectoryName(targetPath));

        this.GetGacPaths();
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name) {
        return AssemblyDefinition.ReadAssembly(Find(name));
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) {
        return AssemblyDefinition.ReadAssembly(Find(name));
    }

    public AssemblyDefinition Resolve(string fullName) {
        return AssemblyDefinition.ReadAssembly(Find(fullName));
    }


    public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) {
        return AssemblyDefinition.ReadAssembly(Find(fullName));
    }

    private void GetGacPaths() {
        this.gacPaths = this.GetDefaultWindowsGacPaths().ToList();
    }

    private IEnumerable<string> GetDefaultWindowsGacPaths() {
        string environmentVariable = Environment.GetEnvironmentVariable("WINDIR");
        if (environmentVariable != null) {
            yield return Path.Combine(environmentVariable, "assembly");
            yield return Path.Combine(environmentVariable, Path.Combine("Microsoft.NET", "assembly"));
        }
    }

    public string GetProgramFilesPath() {
        string programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
        if (programFiles == null)
            return Environment.GetEnvironmentVariable("ProgramFiles");

        return programFiles;
    }


    private string SearchDirectory(string name) {
        foreach (string directory in this.directories) {
            string dllFile = Path.Combine(directory, name + ".dll");
            if (File.Exists(dllFile))
                return dllFile;
            string exeFile = Path.Combine(directory, name + ".exe");
            if (File.Exists(exeFile))
                return exeFile;
        }
        return null;
    }


    public string Find(AssemblyNameReference assemblyNameReference) {
        string file = this.SearchDirectory(assemblyNameReference.Name);
        if (file == null)
            file = this.GetAssemblyInGac(assemblyNameReference);
        if (file != null)
            return file;
        throw new FileNotFoundException();
    }

    public string Find(string assemblyName) {
        string file = this.SearchDirectory(assemblyName);
        if (file != null)
            return file;
        throw new FileNotFoundException();
    }

    private string GetAssemblyInGac(AssemblyNameReference reference) {
        if ((reference.PublicKeyToken == null) || (reference.PublicKeyToken.Length == 0))
            return null;
        return this.GetAssemblyInNetGac(reference);
    }

    private string GetAssemblyInNetGac(AssemblyNameReference reference) {
        var gacs = new[] { "GAC_MSIL", "GAC_32", "GAC" };
        var prefixes = new[] { string.Empty, "v4.0_" };

        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < gacs.Length; j++) {
                string gac = Path.Combine(this.gacPaths[i], gacs[j]);
                string file = GetAssemblyFile(reference, prefixes[i], gac);
                if (Directory.Exists(gac) && File.Exists(file))
                    return file;
            }
        }

        return null;
    }


    private static string GetAssemblyFile(AssemblyNameReference reference, string prefix, string gac) {
        var builder = new StringBuilder();
        builder.Append(prefix);
        builder.Append(reference.Version);
        builder.Append("__");
        for (int i = 0; i < reference.PublicKeyToken.Length; i++) {
            builder.Append(reference.PublicKeyToken[i].ToString("x2"));
        }
        return Path.Combine(
            Path.Combine(Path.Combine(gac, reference.Name), builder.ToString()),
            reference.Name + ".dll");
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}