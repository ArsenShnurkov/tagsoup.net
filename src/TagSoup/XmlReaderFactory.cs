namespace TagSoup.Net {
  using System;
  using System.Configuration;

  using Sax.Net;
  using Sax.Net.Helpers;

  public class XmlReaderFactory : BaseXmlReaderFactory {
    public override IXmlReader CreateXmlReader() {
      var parser = new Parser();
      var section = ConfigurationManager.GetSection("tagsoup") as TagSoupConfigurationSection;
      if (section != null) {
        foreach (NameValueConfigurationElement feature in section.Features) {
          parser.SetFeature(feature.Name, Convert.ToBoolean(feature.Value));
        }
        foreach (NameValueConfigurationElement property in section.Properties) {
          object value = property.Value;
          string typeName = property.Value;
          Type type = Type.GetType(typeName);
          if (type != null) {
            value = Activator.CreateInstance(type);
          }
          parser.SetProperty(property.Name, value);
        }
      }
      return parser;
    }
  }
}
