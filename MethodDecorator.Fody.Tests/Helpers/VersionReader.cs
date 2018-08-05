using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

public class VersionReader {
    public decimal FrameworkVersionAsNumber;
    public string FrameworkVersionAsString;
    public bool IsSilverlight;
    public string TargetFrameworkProfile;

    public VersionReader(string projectPath) {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();
        GetTargetFrameworkIdentifier(xDocument);
        GetFrameworkVersion(xDocument);
        GetTargetFrameworkProfile(xDocument);
    }

    private void GetFrameworkVersion(XDocument xDocument) {
        FrameworkVersionAsString = xDocument.Descendants("TargetFrameworkVersion")
            .Select(c => c.Value)
            .First();
        FrameworkVersionAsNumber = decimal.Parse(
            FrameworkVersionAsString.Substring(1, 3),
            CultureInfo.InvariantCulture);
    }


    private void GetTargetFrameworkProfile(XDocument xDocument) {
        TargetFrameworkProfile = xDocument.Descendants("TargetFrameworkProfile")
            .Select(c => c.Value)
            .FirstOrDefault();
    }

    private void GetTargetFrameworkIdentifier(XDocument xDocument) {
        var targetFrameworkIdentifier = xDocument.Descendants("TargetFrameworkIdentifier")
            .Select(c => c.Value)
            .FirstOrDefault();
        if (string.Equals(targetFrameworkIdentifier, "Silverlight", StringComparison.OrdinalIgnoreCase))
            IsSilverlight = true;
    }
}