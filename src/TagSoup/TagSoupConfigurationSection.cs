namespace TagSoup.Net {
  using System.Configuration;

  public class TagSoupConfigurationSection : ConfigurationSection {
    [ConfigurationProperty("features")]
    public NameValueConfigurationCollection Features {
      get { return (NameValueConfigurationCollection)this["features"]; }
    }

    [ConfigurationProperty("properties")]
    public NameValueConfigurationCollection Properties {
      get { return (NameValueConfigurationCollection)this["properties"]; }
    }
  }
}
