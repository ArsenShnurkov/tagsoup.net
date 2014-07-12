# TagSoup.Net

TagSoup.Net is a .NET port of the TagSoup (http://home.ccil.org/~cowan/tagsoup/) HTML Parser.

TagSoup.Net is a SAX2-compliant parser that parses HTML.

## Installation

To install [TagSoup.Net](https://www.nuget.org/packages/tagsoup.net) from the [NuGet Gallery](http://www.nuget.org), run the following in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```powershell
PM> Install-Package TagSoup.Net
```

# Configuration

To use TagSoup.Net with SAX.Net set `xmlReaderFactoryType` to `TagSoup.Net.TagSoupXmlReaderFactory, TagSoup.Net`

``` XML
<configSections>
  <section name="sax" type="Sax.Net.Helpers.SaxConfigurationSection, Sax.Net"/>
</configSections>
<sax xmlReaderFactoryType="TagSoup.Net.XmlReaderFactory, TagSoup.Net"/>
```

TagSoup.Net features can be changed in the configuration file
```XML
<configSections>
    <section name="tagsoup" type="TagSoup.Net.TagSoupConfigurationSection, TagSoup.Net"/>
</configSections>
<tagsoup>
    <features>
        <add name="http://xml.org/sax/features/namespaces" value="false"/>
        <add name="http://www.ccil.org/~cowan/tagsoup/features/ignore-bogons" value="true"/>
    </features>
</tagsoup>
```
For a list of feature flags see [Parser.cs](src/TagSoup/Parser.cs)

# Usage

See [SAX.Net](http://www.github.com/rasmusjp/sax.net#usage)


# License

TagSoup.Net is licensed under [LGPL V3](LICENSE).
