using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public static class XmlExtensions {
    public static void StripNamespace(this XDocument document) {
        if (document.Root == null)
            return;
        foreach (XElement element in document.Root.DescendantsAndSelf()) {
            element.Name = element.Name.LocalName;
            element.ReplaceAttributes(GetAttributes(element).ToList());
        }
    }

    private static IEnumerable<XAttribute> GetAttributes(XElement xElement) {
        return xElement.Attributes()
            .Where(x => !x.IsNamespaceDeclaration)
            .Select(x => new XAttribute(x.Name.LocalName, x.Value));
    }
}