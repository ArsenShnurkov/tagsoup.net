namespace TagSoup.Net {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;

  using Sax.Net;
  using Sax.Net.Ext;
  using Sax.Net.Helpers;

  /// <summary>
  ///   The SAX parser class.
  /// </summary>
  public class Parser : DefaultHandler, IScanHandler, IXmlReader, ILexicalHandler {
    // Default values for feature flags

    private const bool DEFAULT_NAMESPACES = true;
    private const bool DEFAULT_IGNORE_BOGONS = false;
    private const bool DEFAULT_BOGONS_EMPTY = false;
    private const bool DEFAULT_ROOT_BOGONS = true;
    private const bool DEFAULT_DEFAULT_ATTRIBUTES = true;
    private const bool DEFAULT_TRANSLATE_COLONS = false;
    private const bool DEFAULT_RESTART_ELEMENTS = true;
    private const bool DEFAULT_IGNORABLE_WHITESPACE = false;
    private const bool DEFAULT_CDATA_ELEMENTS = true;

    // Feature flags.

    /// <summary>
    ///   A value of "true" indicates namespace URIs and unprefixed local
    ///   names for element and attribute names will be available.
    /// </summary>
    public const string NAMESPACES_FEATURE = "http://xml.org/sax/features/namespaces";

    /// <summary>
    ///   A value of "true" indicates that XML qualified names (with prefixes)
    ///   and attributes (including xmlns* attributes) will be available.
    ///   We don't support this value.
    /// </summary>
    public const string NAMESPACE_PREFIXES_FEATURE = "http://xml.org/sax/features/namespace-prefixes";

    /// <summary>
    ///   Reports whether this parser processes external general entities
    ///   (it doe
    /// </summary>
    public const string EXTERNAL_GENERAL_ENTITIES_FEATURE = "http://xml.org/sax/features/external-general-entities";

    /// <summary>
    ///   Reports whether this parser processes external parameter entities
    ///   (it doesn't).
    /// </summary>
    public const string EXTERNAL_PARAMETER_ENTITIES_FEATURE = "http://xml.org/sax/features/external-parameter-entities";

    /// <summary>
    ///   May be examined only during a parse, after the startDocument()
    ///   callback has been completed; read-only. The value is true if
    ///   the document specified standalone="yes" in its XML declaration,
    ///   and otherwise is false.  (It's always false.)
    /// </summary>
    public const string IS_STANDALONE_FEATURE = "http://xml.org/sax/features/is-standalone";

    /// <summary>
    ///   A value of "true" indicates that the LexicalHandler will report
    ///   the beginning and end of parameter entities (it won't).
    /// </summary>
    public const string LEXICAL_HANDLER_PARAMETER_ENTITIES_FEATURE =
      "http://xml.org/sax/features/lexical-handler/parameter-entities";

    /// <summary>
    ///   A value of "true" indicates that system IDs in declarations will
    ///   be absolutized (relative to their base URIs) before reporting.
    ///   (This returns true but doesn't actually do anything.)
    /// </summary>
    public const string RESOLVE_DTD_URIS_FEATURE = "http://xml.org/sax/features/resolve-dtd-uris";

    /// <summary>
    ///   Has a value of "true" if all XML names (for elements,
    ///   prefixes, attributes, entities, notations, and local
    ///   names), as well as Namespace URIs, will have been interned
    ///   using <see cref="string.Intern" />. This supports fast testing of
    ///   equality/inequality against string constants, rather than forcing
    ///   slower calls to <see cref="string.Equals(object)" />.  (We always intern.)
    /// </summary>
    public const string STRING_INTERNING_FEATURE = "http://xml.org/sax/features/string-interning";

    /// <summary>
    ///   Returns "true" if the Attributes objects passed by this
    ///   parser in <see cref="IContentHandler.StartElement" /> implement the
    ///   <see cref="Sax.Net.Ext.IAttributes2" /> interface.	(They don't.)
    /// </summary>
    public const string USE_ATTRIBUTES2_FEATURE = "http://xml.org/sax/features/use-attributes2";

    /// <summary>
    ///   Returns "true" if the Locator objects passed by this parser
    ///   parser in <see cref="IContentHandler.SetDocumentLocator" /> implement the
    ///   <see cref="Sax.Net.Ext.ILocator2" /> interface.  (They don't.)
    /// </summary>
    public const string USE_LOCATOR2_FEATURE = "http://xml.org/sax/features/use-locator2";
    /// <summary>
    ///   Returns "true" if, when setEntityResolver is given an object
    ///   implementing the  <see cref="Sax.Net.Ext.IEntityResolver2" /> interface,
    ///   those new methods will be used.  (They won't be.)
    /// </summary>
    public const string USE_ENTITY_RESOLVER2_FEATURE = "http://xml.org/sax/features/use-entity-resolver2";

    /// <summary>
    ///   Controls whether the parser is reporting all validity errors
    ///   (We don't report any validity errors.)
    /// </summary>
    public const string VALIDATION_FEATURE = "http://xml.org/sax/features/validation";

    /// <summary>
    ///   Controls whether the parser reports Unicode normalization
    ///   errors as described in section 2.13 and Appendix B of the XML
    ///   1.1 Recommendation.  (We don't normalize.)
    /// </summary>
    public const string UNICODE_NORMALIZATION_CHECKING_FEATURE =
      "http://xml.org/sax/features/unicode-normalization-checking";

    /// <summary>
    ///   Controls whether, when the namespace-prefixes feature is set,
    ///   the parser treats namespace declaration attributes as being in
    ///   the http://www.w3.org/2000/xmlns/ namespace.  (It doesn't.)
    /// </summary>
    public const string XMLNS_URIS_FEATURE = "http://xml.org/sax/features/xmlns-uris";

    /// <summary>
    ///   Returns <c>true</c> if the parser supports both XML 1.1 and XML 1.0.
    ///   (Always <c>false</c>.)
    /// </summary>
    public const string XML11_FEATURE = "http://xml.org/sax/features/xml-1.1";

    /// <summary>
    ///   A value of <c>true</c> indicates that the parser will ignore
    ///   unknown elements.
    /// </summary>
    public const string IGNORE_BOGONS_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/ignore-bogons";

    /// <summary>
    ///   A value of <c>true</c> indicates that the parser will give unknown
    ///   elements a content model of EMPTY; a value of <c>false</c>, a
    ///   content model of ANY.
    /// </summary>
    public const string BOGONS_EMPTY_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/bogons-empty";

    /// <summary>
    ///   A value of <c>true</c> indicates that the parser will allow unknown
    ///   elements to be the root element.
    /// </summary>
    public const string ROOT_BOGONS_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/root-bogons";

    /// <summary>
    ///   A value of <c>true</c> indicates that the parser will return default
    ///   attribute values for missing attributes that have default values.
    /// </summary>
    public const string DEFAULT_ATTRIBUTES_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/default-attributes";

    /// <summary>
    ///   A value of <c>true</c> indicates that the parser will
    ///   translate colons into underscores in names.
    /// </summary>
    public const string TRANSLATE_COLONS_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/translate-colons";

    /// <summary>
    ///   A value of <c>true</c> indicates that the parser will
    ///   attempt to restart the restartable elements.
    /// </summary>
    public const string RESTART_ELEMENTS_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/restart-elements";

    /// <summary>
    ///   A value of "true" indicates that the parser will
    ///   transmit whitespace in element-only content via the SAX
    ///   ignorableWhitespace callback.  Normally this is not done,
    ///   because HTML is an SGML application and SGML suppresses
    ///   such whitespace.
    /// </summary>
    public const string IGNORABLE_WHITESPACE_FEATURE =
      "http://www.ccil.org/~cowan/tagsoup/features/ignorable-whitespace";

    /// <summary>
    ///   A value of "true" indicates that the parser will treat CDATA
    ///   elements specially.  Normally true, since the input is by
    ///   default HTML.
    /// </summary>
    public const string CDATA_ELEMENTS_FEATURE = "http://www.ccil.org/~cowan/tagsoup/features/cdata-elements";

    /// <summary>
    ///   Used to see some syntax events that are essential in some
    ///   applications: comments, CDATA delimiters, selected general
    ///   entity inclusions, and the start and end of the DTD (and
    ///   declaration of document element name). The Object must implement
    ///   <see cref="ILexicalHandler" />
    /// </summary>
    public const string LEXICAL_HANDLER_PROPERTY = "http://xml.org/sax/properties/lexical-handler";

    /// <summary>
    ///   Specifies the Scanner object this Parser uses.
    /// </summary>
    public const string SCANNER_PROPERTY = "http://www.ccil.org/~cowan/tagsoup/properties/scanner";

    /// <summary>
    ///   Specifies the Schema object this Parser uses.
    /// </summary>
    public const string SCHEMA_PROPERTY = "http://www.ccil.org/~cowan/tagsoup/properties/schema";

    /// <summary>
    ///   Specifies the AutoDetector (for encoding detection) this Parser uses.
    /// </summary>
    public const string AUTO_DETECTOR_PROPERTY = "http://www.ccil.org/~cowan/tagsoup/properties/auto-detector";
    private const string LEGAL = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-'()+,./:=?;!*#@$_%";
    private static readonly char[] Etagchars = { '<', '/', '>' };

    // Due to sucky Java order of initialization issues, these
    // entries are maintained separately from the initial values of
    // the corresponding instance variables, but care must be taken
    // to keep them in sync.

    private readonly Hashtable _features = new Hashtable {
      { NAMESPACES_FEATURE, DEFAULT_NAMESPACES },
      { NAMESPACE_PREFIXES_FEATURE, false },
      { EXTERNAL_GENERAL_ENTITIES_FEATURE, false },
      { EXTERNAL_PARAMETER_ENTITIES_FEATURE, false },
      { IS_STANDALONE_FEATURE, false },
      { LEXICAL_HANDLER_PARAMETER_ENTITIES_FEATURE, false },
      { RESOLVE_DTD_URIS_FEATURE, true },
      { STRING_INTERNING_FEATURE, true },
      { USE_ATTRIBUTES2_FEATURE, false },
      { USE_LOCATOR2_FEATURE, false },
      { USE_ENTITY_RESOLVER2_FEATURE, false },
      { VALIDATION_FEATURE, false },
      { XMLNS_URIS_FEATURE, false },
      { XML11_FEATURE, false },
      { IGNORE_BOGONS_FEATURE, DEFAULT_IGNORE_BOGONS },
      { BOGONS_EMPTY_FEATURE, DEFAULT_BOGONS_EMPTY },
      { ROOT_BOGONS_FEATURE, DEFAULT_ROOT_BOGONS },
      { DEFAULT_ATTRIBUTES_FEATURE, DEFAULT_DEFAULT_ATTRIBUTES },
      { TRANSLATE_COLONS_FEATURE, DEFAULT_TRANSLATE_COLONS },
      { RESTART_ELEMENTS_FEATURE, DEFAULT_RESTART_ELEMENTS },
      { IGNORABLE_WHITESPACE_FEATURE, DEFAULT_IGNORABLE_WHITESPACE },
      { CDATA_ELEMENTS_FEATURE, DEFAULT_CDATA_ELEMENTS },
    };
    private string _attributeName;
    private IAutoDetector _autoDetector;
    private bool _bogonsEmpty = DEFAULT_BOGONS_EMPTY;
    private bool _cdataElements = DEFAULT_CDATA_ELEMENTS;
    private IContentHandler _contentHandler;
    private bool _defaultAttributes = DEFAULT_DEFAULT_ATTRIBUTES;
    private bool _doctypeIsPresent;
    private string _doctypeName;
    private string _doctypePublicId;
    private string _doctypeSystemId;
    private IDTDHandler _dtdHandler;
    private int _entity; // needs to support chars past U+FFFF
    private IEntityResolver _entityResolver;
    private IErrorHandler _errorHandler;
    private bool _ignorableWhitespace = DEFAULT_IGNORABLE_WHITESPACE;
    private bool _ignoreBogons = DEFAULT_IGNORE_BOGONS;
    private ILexicalHandler _lexicalHandler;
    private bool _namespaces = DEFAULT_NAMESPACES;
    private Element _newElement;
    private Element _pcdata;
    private string _piTarget;
    private bool _restartElements = DEFAULT_RESTART_ELEMENTS;
    private bool _rootBogons = DEFAULT_ROOT_BOGONS;
    private Element _saved;
    private IScanner _scanner;
    private Schema _schema;
    private Element _stack;
    private char[] _theCommentBuffer = new char[2000];
    private bool _translateColons = DEFAULT_TRANSLATE_COLONS;
    private bool _virginStack = true;

    /// <summary>
    ///   Creates a new instance of <see cref="Parser" />
    /// </summary>
    public Parser() {
      _newElement = null;
      _contentHandler = this;
      _lexicalHandler = this;
      _dtdHandler = this;
      _errorHandler = this;
      _entityResolver = this;
    }

    public void Comment(char[] ch, int start, int length) {
    }

    public void EndCDATA() {
    }

    public void EndDTD() {
    }

    public void EndEntity(string name) {
    }

    public void StartCDATA() {
    }

    public void StartDTD(string name, string publicid, string systemid) {
    }

    public void StartEntity(string name) {
    }

    public void Adup(char[] buff, int offset, int length) {
      if (_newElement == null || _attributeName == null) {
        return;
      }
      _newElement.SetAttribute(_attributeName, null, _attributeName);
      _attributeName = null;
    }

    public void Aname(char[] buff, int offset, int length) {
      if (_newElement == null) {
        return;
      }
      // Currently we don't rely on Schema to canonicalize
      // attribute names.
      _attributeName = MakeName(buff, offset, length).ToLowerInvariant();
      //		System.err.println("%% Attribute name " + theAttributeName);
    }

    public void Aval(char[] buff, int offset, int length) {
      if (_newElement == null || _attributeName == null) {
        return;
      }
      var value = new string(buff, offset, length);
      //		System.err.println("%% Attribute value [" + value + "]");
      value = ExpandEntities(value);
      _newElement.SetAttribute(_attributeName, null, value);
      _attributeName = null;
      //		System.err.println("%% Aval done");
    }

    public void Entity(char[] buff, int offset, int length) {
      _entity = LookupEntity(buff, offset, length);
    }

    public void EOF(char[] buff, int offset, int length) {
      if (_virginStack) {
        Rectify(_pcdata);
      }
      while (_stack.Next != null) {
        Pop();
      }
      if (!(_schema.Uri.Equals(""))) {
        _contentHandler.EndPrefixMapping(_schema.Prefix);
      }
      _contentHandler.EndDocument();
    }

    public void ETag(char[] buff, int offset, int length) {
      if (ETagCdata(buff, offset, length)) {
        return;
      }
      ETagBasic(buff, offset, length);
    }

    /// <summary>
    ///   Parsing the complete XML Document Type Definition is way too complex,
    ///   but for many simple cases we can extract something useful from it.
    ///   doctypedecl ::= '&lt;!DOCTYPE' S Name (S ExternalID)? S? ('[' intSubset ']' S?)? '>'
    ///   DeclSep ::= PEReference | S
    ///   intSubset ::= (markupdecl | DeclSep)*
    ///   markupdecl ::= elementdecl | AttlistDecl | EntityDecl | NotationDecl | PI | Comment
    ///   ExternalID ::= 'SYSTEM' S SystemLiteral | 'PUBLIC' S PubidLiteral S SystemLiteral
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    public void Decl(char[] buff, int offset, int length) {
      var s = new string(buff, offset, length);
      string name = null;
      string systemid = null;
      string publicid = null;
      string[] v = Split(s);
      if (v.Length > 0 && "DOCTYPE".Equals(v[0], StringComparison.OrdinalIgnoreCase)) {
        if (_doctypeIsPresent) {
          return; // one doctype only!
        }
        _doctypeIsPresent = true;
        if (v.Length > 1) {
          name = v[1];
          if (v.Length > 3 && "SYSTEM".Equals(v[2])) {
            systemid = v[3];
          } else if (v.Length > 3 && "PUBLIC".Equals(v[2])) {
            publicid = v[3];
            if (v.Length > 4) {
              systemid = v[4];
            } else {
              systemid = "";
            }
          }
        }
      }
      publicid = TrimQuotes(publicid);
      systemid = TrimQuotes(systemid);
      if (name != null) {
        publicid = CleanPublicid(publicid);
        _lexicalHandler.StartDTD(name, publicid, systemid);
        _lexicalHandler.EndDTD();
        _doctypeName = name;
        _doctypePublicId = publicid;
        var locator = _scanner as ILocator;
        if (locator != null) {
          // Must resolve systemid
          _doctypeSystemId = locator.SystemId;
          try {
            if (Uri.IsWellFormedUriString(_doctypeSystemId, UriKind.Absolute)) {
              _doctypeSystemId = new Uri(new Uri(_doctypeSystemId), systemid).ToString();
            }
          } catch (Exception e) {
          }
        }
      }
    }

    public void GI(char[] buff, int offset, int length) {
      if (_newElement != null) {
        return;
      }
      string name = MakeName(buff, offset, length);
      if (name == null) {
        return;
      }
      ElementType type = _schema.GetElementType(name);
      if (type == null) {
        // Suppress unknown elements if ignore-bogons is on
        if (_ignoreBogons) {
          return;
        }
        int bogonModel = (_bogonsEmpty ? Schema.M_EMPTY : Schema.M_ANY);
        int bogonMemberOf = (_rootBogons ? Schema.M_ANY : (Schema.M_ANY & ~ Schema.M_ROOT));
        _schema.ElementType(name, bogonModel, bogonMemberOf, 0);
        if (!_rootBogons) {
          _schema.Parent(name, _schema.RootElementType.Name);
        }
        type = _schema.GetElementType(name);
      }

      _newElement = new Element(type, _defaultAttributes);
      //		System.err.println("%% Got GI " + theNewElement.name());
    }

    public void CDSect(char[] buff, int offset, int length) {
      _lexicalHandler.StartCDATA();
      PCDATA(buff, offset, length);
      _lexicalHandler.EndCDATA();
    }

    public void PCDATA(char[] buff, int offset, int length) {
      if (length == 0) {
        return;
      }
      bool allWhite = true;
      for (int i = 0; i < length; i++) {
        if (!char.IsWhiteSpace(buff[offset + i])) {
          allWhite = false;
        }
      }
      if (allWhite && !_stack.CanContain(_pcdata)) {
        if (_ignorableWhitespace) {
          _contentHandler.IgnorableWhitespace(buff, offset, length);
        }
      } else {
        Rectify(_pcdata);
        _contentHandler.Characters(buff, offset, length);
      }
    }

    public void PITarget(char[] buff, int offset, int length) {
      if (_newElement != null) {
        return;
      }
      _piTarget = MakeName(buff, offset, length).Replace(':', '_');
    }

    public void PI(char[] buff, int offset, int length) {
      if (_newElement != null || _piTarget == null) {
        return;
      }
      if ("xml".Equals(_piTarget, StringComparison.OrdinalIgnoreCase)) {
        return;
      }
      //		if (length > 0 && buff[length - 1] == '?') System.err.println("%% Removing ? from PI");
      if (length > 0 && buff[length - 1] == '?') {
        length--; // remove trailing ?
      }
      _contentHandler.ProcessingInstruction(_piTarget, new string(buff, offset, length));
      _piTarget = null;
    }

    public void STagc(char[] buff, int offset, int length) {
      //		System.err.println("%% Start-tag");
      if (_newElement == null) {
        return;
      }
      Rectify(_newElement);
      if (_stack.Model == Schema.M_EMPTY) {
        // Force an immediate end tag
        ETagBasic(buff, offset, length);
      }
    }

    public void STage(char[] buff, int offset, int length) {
      //		System.err.println("%% Empty-tag");
      if (_newElement == null) {
        return;
      }
      Rectify(_newElement);
      // Force an immediate end tag
      ETagBasic(buff, offset, length);
    }

    public void Cmnt(char[] buff, int offset, int length) {
      _lexicalHandler.Comment(buff, offset, length);
    }

    public int GetEntity() {
      return _entity;
    }

    public bool GetFeature(string name) {
      if (_features.ContainsKey(name)) {
        return (bool)_features[name];
      }
      throw new SAXNotRecognizedException("Unknown feature " + name);
    }

    public void SetFeature(string name, bool value) {
      if (false == _features.ContainsKey(name)) {
        throw new SAXNotRecognizedException("Unknown feature " + name);
      }
      _features[name] = value;

      if (name.Equals(NAMESPACES_FEATURE)) {
        _namespaces = value;
      } else if (name.Equals(IGNORE_BOGONS_FEATURE)) {
        _ignoreBogons = value;
      } else if (name.Equals(BOGONS_EMPTY_FEATURE)) {
        _bogonsEmpty = value;
      } else if (name.Equals(ROOT_BOGONS_FEATURE)) {
        _rootBogons = value;
      } else if (name.Equals(DEFAULT_ATTRIBUTES_FEATURE)) {
        _defaultAttributes = value;
      } else if (name.Equals(TRANSLATE_COLONS_FEATURE)) {
        _translateColons = value;
      } else if (name.Equals(RESTART_ELEMENTS_FEATURE)) {
        _restartElements = value;
      } else if (name.Equals(IGNORABLE_WHITESPACE_FEATURE)) {
        _ignorableWhitespace = value;
      } else if (name.Equals(CDATA_ELEMENTS_FEATURE)) {
        _cdataElements = value;
      }
    }

    public object GetProperty(string name) {
      if (name.Equals(LEXICAL_HANDLER_PROPERTY)) {
        return _lexicalHandler == this ? null : _lexicalHandler;
      }
      if (name.Equals(SCANNER_PROPERTY)) {
        return _scanner;
      }
      if (name.Equals(SCHEMA_PROPERTY)) {
        return _schema;
      }
      if (name.Equals(AUTO_DETECTOR_PROPERTY)) {
        return _autoDetector;
      }
      throw new SAXNotRecognizedException("Unknown property " + name);
    }

    public void SetProperty(string name, object value) {
      if (name.Equals(LEXICAL_HANDLER_PROPERTY)) {
        if (value == null) {
          _lexicalHandler = this;
        } else {
          var handler = value as ILexicalHandler;
          if (handler != null) {
            _lexicalHandler = handler;
          } else {
            throw new SAXNotSupportedException("Your lexical handler is not a ILexicalHandler");
          }
        }
      } else if (name.Equals(SCANNER_PROPERTY)) {
        var scanner = value as IScanner;
        if (scanner != null) {
          _scanner = scanner;
        } else {
          throw new SAXNotSupportedException("Your scanner is not a IScanner");
        }
      } else if (name.Equals(SCHEMA_PROPERTY)) {
        var schema = value as Schema;
        if (schema != null) {
          _schema = schema;
        } else {
          throw new SAXNotSupportedException("Your schema is not a Schema");
        }
      } else if (name.Equals(AUTO_DETECTOR_PROPERTY)) {
        var detector = value as IAutoDetector;
        if (detector != null) {
          _autoDetector = detector;
        } else {
          throw new SAXNotSupportedException("Your auto-detector is not an IAutoDetector");
        }
      } else {
        throw new SAXNotRecognizedException("Unknown property " + name);
      }
    }

    public IEntityResolver EntityResolver {
      get { return _entityResolver == this ? null : _entityResolver; }
      set { _entityResolver = value ?? this; }
    }

    public IDTDHandler DTDHandler {
      get { return _dtdHandler == this ? null : _dtdHandler; }
      set { _dtdHandler = value ?? this; }
    }

    public IContentHandler ContentHandler {
      get { return _contentHandler == this ? null : _contentHandler; }
      set { _contentHandler = value ?? this; }
    }

    public IErrorHandler ErrorHandler {
      get { return _errorHandler == this ? null : _errorHandler; }
      set { _errorHandler = value ?? this; }
    }

    public void Parse(InputSource input) {
      Setup();
      TextReader r = GetReader(input);
      _contentHandler.StartDocument();
      _scanner.ResetDocumentLocator(input.PublicId, input.SystemId);
      var locator = _scanner as ILocator;
      if (locator != null) {
        _contentHandler.SetDocumentLocator(locator);
      }
      if (!(_schema.Uri.Equals(""))) {
        _contentHandler.StartPrefixMapping(_schema.Prefix, _schema.Uri);
      }
      _scanner.Scan(r, this);
    }

    public void Parse(string systemid) {
      Parse(new InputSource(systemid));
    }

    // Sets up instance variables that haven't been set by setFeature
    private void Setup() {
      if (_schema == null) {
        _schema = new HTMLSchema();
      }
      if (_scanner == null) {
        _scanner = new HTMLScanner();
      }
      if (_autoDetector == null) {
        _autoDetector = new AutoDetectorDelegate(stream => new StreamReader(stream));
      }
      _stack = new Element(_schema.GetElementType("<root>"), _defaultAttributes);
      _pcdata = new Element(_schema.GetElementType("<pcdata>"), _defaultAttributes);
      _newElement = null;
      _attributeName = null;
      _piTarget = null;
      _saved = null;
      _entity = 0;
      _virginStack = true;
      _doctypeName = _doctypePublicId = _doctypeSystemId = null;
    }

    /// <summary>
    ///   Return a Reader based on the contents of an InputSource
    ///   Buffer the Stream
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private TextReader GetReader(InputSource s) {
      TextReader r = s.Reader;
      Stream i = s.Stream;
      Encoding encoding = s.Encoding;
      string publicid = s.PublicId;
      string systemid = s.SystemId;
      if (r == null) {
        if (i == null) {
          i = GetInputStream(publicid, systemid);
        }
        if (!(i is BufferedStream)) {
          i = new BufferedStream(i);
        }
        if (encoding == null) {
          r = _autoDetector.AutoDetectingReader(i);
        } else {
          //try {
          //TODO: Safe?
          r = new StreamReader(i, encoding);
          //  }
          //catch (UnsupportedEncodingException e) {
          //  r = new StreamReader(i);
          //  }
        }
      }
      //		r = new BufferedReader(r);
      return r;
    }

    /// <summary>
    ///   Get an Stream based on a publicid and a systemid
    ///   We don't process publicids (who uses them anyhow?)
    /// </summary>
    /// <param name="publicid"></param>
    /// <param name="systemid"></param>
    /// <returns></returns>
    private Stream GetInputStream(string publicid, string systemid) {
      var basis = new Uri("file://" + Environment.CurrentDirectory + Path.DirectorySeparatorChar);
      var url = new Uri(basis, systemid);
      return new FileStream(url.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    /// <summary>
    ///   Expand entity references in attribute values selectively.
    ///   Currently we expand a reference iff it is properly terminated
    ///   with a semicolon.
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    private string ExpandEntities(string src) {
      int refStart = -1;
      int len = src.Length;
      var dst = new char[len];
      int dstlen = 0;
      for (int i = 0; i < len; i++) {
        char ch = src[i];
        dst[dstlen++] = ch;
        //			System.err.print("i = " + i + ", d = " + dstlen + ", ch = [" + ch + "] ");
        if (ch == '&' && refStart == -1) {
          // start of a ref excluding &
          refStart = dstlen;
          //				System.err.println("start of ref");
        } else if (refStart == -1) {
          // not in a ref
          //				System.err.println("not in ref");
        } else if (char.IsLetter(ch) || char.IsDigit(ch) || ch == '#') {
          // valid entity char
          //				System.err.println("valid");
        } else if (ch == ';') {
          // properly terminated ref
          //				System.err.print("got [" + new string(dst, refStart, dstlen-refStart-1) + "]");
          int ent = LookupEntity(dst, refStart, dstlen - refStart - 1);
          //				System.err.println(" = " + ent);
          if (ent > 0xFFFF) {
            ent -= 0x10000;
            dst[refStart - 1] = (char)((ent >> 10) + 0xD800);
            dst[refStart] = (char)((ent & 0x3FF) + 0xDC00);
            dstlen = refStart + 1;
          } else if (ent != 0) {
            dst[refStart - 1] = (char)ent;
            dstlen = refStart;
          }
          refStart = -1;
        } else {
          // improperly terminated ref
          //				System.err.println("end of ref");
          refStart = -1;
        }
      }
      return new string(dst, 0, dstlen);
    }

    /// <summary>
    ///   Process numeric character references,
    ///   deferring to the schema for named ones.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private int LookupEntity(char[] buff, int offset, int length) {
      int result = 0;
      if (length < 1) {
        return result;
      }
      //		System.err.println("%% Entity at " + offset + " " + length);
      //		System.err.println("%% Got entity [" + new string(buff, offset, length) + "]");
      if (buff[offset] == '#') {
        if (length > 1 && (buff[offset + 1] == 'x' || buff[offset + 1] == 'X')) {
          try {
            return Convert.ToInt32(new string(buff, offset + 2, length - 2), 16);
          } catch (FormatException e) {
            return 0;
          }
        }
        try {
          return Convert.ToInt32(new string(buff, offset + 1, length - 1), 10);
        } catch (FormatException e) {
          return 0;
        }
      }
      return _schema.GetEntity(new string(buff, offset, length));
    }

    public bool ETagCdata(char[] buff, int offset, int length) {
      string currentName = _stack.Name;
      // If this is a CDATA element and the tag doesn't match,
      // or isn't properly formed (junk after the name),
      // restart CDATA mode and process the tag as characters.
      if (_cdataElements && (_stack.Flags & Schema.F_CDATA) != 0) {
        bool realTag = (length == currentName.Length);
        if (realTag) {
          for (int i = 0; i < length; i++) {
            if (char.ToLower(buff[offset + i]) != char.ToLower(currentName[i])) {
              realTag = false;
              break;
            }
          }
        }
        if (!realTag) {
          _contentHandler.Characters(Etagchars, 0, 2);
          _contentHandler.Characters(buff, offset, length);
          _contentHandler.Characters(Etagchars, 2, 1);
          _scanner.StartCDATA();
          return true;
        }
      }
      return false;
    }

    public void ETagBasic(char[] buff, int offset, int length) {
      _newElement = null;
      string name;
      if (length != 0) {
        // Canonicalize case of name
        name = MakeName(buff, offset, length);
        //			System.err.println("got etag [" + name + "]");
        ElementType type = _schema.GetElementType(name);
        if (type == null) {
          return; // mysterious end-tag
        }
        name = type.Name;
      } else {
        name = _stack.Name;
      }
      //		System.err.println("%% Got end of " + name);

      Element sp;
      bool inNoforce = false;
      for (sp = _stack; sp != null; sp = sp.Next) {
        if (sp.Name.Equals(name)) {
          break;
        }
        if ((sp.Flags & Schema.F_NOFORCE) != 0) {
          inNoforce = true;
        }
      }

      if (sp == null) {
        return; // Ignore unknown etags
      }
      if (sp.Next == null || sp.Next.Next == null) {
        return;
      }
      if (inNoforce) {
        // inside an F_NOFORCE element?
        sp.Preclose(); // preclose the matching element
      } else {
        // restartably pop everything above us
        while (_stack != sp) {
          RestartablyPop();
        }
        Pop();
      }
      // pop any preclosed elements now at the top
      while (_stack.IsPreclosed) {
        Pop();
      }
      Restart(null);
    }

    /// <summary>
    ///   Push restartables on the stack if possible
    ///   e is the next element to be started, if we know what it is
    /// </summary>
    /// <param name="e"></param>
    private void Restart(Element e) {
      while (_saved != null && _stack.CanContain(_saved) && (e == null || _saved.CanContain(e))) {
        Element next = _saved.Next;
        Push(_saved);
        _saved = next;
      }
    }

    /// <summary>
    ///   Pop the stack irrevocably
    /// </summary>
    private void Pop() {
      if (_stack == null) {
        return; // empty stack
      }
      string name = _stack.Name;
      string localName = _stack.LocalName;
      string ns = _stack.Namespace;
      string prefix = PrefixOf(name);

      //		System.err.println("%% Popping " + name);
      if (!_namespaces) {
        ns = localName = "";
      }
      _contentHandler.EndElement(ns, localName, name);
      if (Foreign(prefix, ns)) {
        _contentHandler.EndPrefixMapping(prefix);
        //			System.err.println("%% Unmapping [" + prefix + "] for elements to " + namespace);
      }
      Attributes atts = _stack.Attributes;
      for (int i = atts.Length - 1; i >= 0; i--) {
        string attNamespace = atts.GetUri(i);
        string attPrefix = PrefixOf(atts.GetQName(i));
        if (Foreign(attPrefix, attNamespace)) {
          _contentHandler.EndPrefixMapping(attPrefix);
          //			System.err.println("%% Unmapping [" + attPrefix + "] for attributes to " + attNamespace);
        }
      }
      _stack = _stack.Next;
    }

    /// <summary>
    ///   Pop the stack restartably
    /// </summary>
    private void RestartablyPop() {
      Element popped = _stack;
      Pop();
      if (_restartElements && (popped.Flags & Schema.F_RESTART) != 0) {
        popped.Anonymize();
        popped.Next = _saved;
        _saved = popped;
      }
    }

    /// <summary>
    ///   Push element onto stack
    /// </summary>
    /// <param name="e"></param>
    private void Push(Element e) {
      string name = e.Name;
      string localName = e.LocalName;
      string ns = e.Namespace;
      string prefix = PrefixOf(name);

      //		System.err.println("%% Pushing " + name);
      e.Clean();
      if (!_namespaces) {
        ns = localName = "";
      }
      if (_virginStack && localName.Equals(_doctypeName, StringComparison.OrdinalIgnoreCase)) {
        try {
          _entityResolver.ResolveEntity(_doctypePublicId, _doctypeSystemId);
        } catch (IOException ew) {
        } // Can't be thrown for root I believe.
      }
      if (Foreign(prefix, ns)) {
        _contentHandler.StartPrefixMapping(prefix, ns);
        //			System.err.println("%% Mapping [" + prefix + "] for elements to " + namespace);
      }
      Attributes atts = e.Attributes;
      int len = atts.Length;
      for (int i = 0; i < len; i++) {
        string attNamespace = atts.GetUri(i);
        string attPrefix = PrefixOf(atts.GetQName(i));
        if (Foreign(attPrefix, attNamespace)) {
          _contentHandler.StartPrefixMapping(attPrefix, attNamespace);
          //				System.err.println("%% Mapping [" + attPrefix + "] for attributes to " + attNamespace);
        }
      }
      _contentHandler.StartElement(ns, localName, name, e.Attributes);
      e.Next = _stack;
      _stack = e;
      _virginStack = false;
      if (_cdataElements && (_stack.Flags & Schema.F_CDATA) != 0) {
        _scanner.StartCDATA();
      }
    }

    /// <summary>
    ///   Get the prefix from a QName
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string PrefixOf(string name) {
      int i = name.IndexOf(':');
      string prefix = "";
      if (i != -1) {
        prefix = name.Substring(0, i);
      }
      //		System.err.println("%% " + prefix + " is prefix of " + name);
      return prefix;
    }

    /// <summary>
    ///   Return true if we have a foreign name
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="ns"></param>
    /// <returns></returns>
    private bool Foreign(string prefix, string ns) {
      //		System.err.print("%% Testing " + prefix + " and " + namespace + " for foreignness -- ");
      bool foreign = !(prefix.Equals("") || ns.Equals("") || ns.Equals(_schema.Uri));
      //		System.err.println(foreign);
      return foreign;
    }

    // If the string is quoted, trim the quotes.
    private static string TrimQuotes(string value) {
      if (value == null) {
        return null;
      }
      int length = value.Length;
      if (length == 0) {
        return value;
      }
      char s = value[0];
      char e = value[length - 1];
      if (s == e && (s == '\'' || s == '"')) {
        value = value.Substring(1, value.Length - 1);
      }
      return value;
    }

    /// <summary>
    ///   Split the supplied string into words or phrases seperated by spaces.
    ///   Recognises quotes around a phrase and doesn't split it.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    private static string[] Split(string val) {
      val = val.Trim();
      if (val.Length == 0) {
        return new string[0];
      }
      var l = new List<string>();
      int s = 0;
      int e = 0;
      bool sq = false; // single quote
      bool dq = false; // double quote
      var lastc = (char)0;
      int len = val.Length;
      for (e = 0; e < len; e++) {
        char c = val[e];
        if (!dq && c == '\'' && lastc != '\\') {
          sq = !sq;
          if (s < 0) {
            s = e;
          }
        } else if (!sq && c == '\"' && lastc != '\\') {
          dq = !dq;
          if (s < 0) {
            s = e;
          }
        } else if (!sq && !dq) {
          if (char.IsWhiteSpace(c)) {
            if (s >= 0) {
              l.Add(val.Substring(s, e - s));
            }
            s = -1;
          } else if (s < 0 && c != ' ') {
            s = e;
          }
        }
        lastc = c;
      }
      l.Add(val.Substring(s, e - s));
      return l.ToArray();
    }

    /// <summary>
    ///   Replace junk in publicids with spaces
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    private string CleanPublicid(string src) {
      if (src == null) {
        return null;
      }
      int len = src.Length;
      var dst = new StringBuilder(len);
      bool suppressSpace = true;
      for (int i = 0; i < len; i++) {
        char ch = src[i];
        if (LEGAL.IndexOf(ch) != -1) {
          // legal but not whitespace
          dst.Append(ch);
          suppressSpace = false;
        } else if (suppressSpace) {
          // normalizable whitespace or junk
        } else {
          dst.Append(' ');
          suppressSpace = true;
        }
      }
      //		System.err.println("%% Publicid [" + dst.tostring().trim() + "]");
      return dst.ToString().Trim(); // trim any final junk whitespace
    }

    /// <summary>
    ///   Rectify the stack, pushing and popping as needed
    ///   so that the argument can be safely pushed
    /// </summary>
    /// <param name="e"></param>
    private void Rectify(Element e) {
      Element sp;
      while (true) {
        for (sp = _stack; sp != null; sp = sp.Next) {
          if (sp.CanContain(e)) {
            break;
          }
        }
        if (sp != null) {
          break;
        }
        ElementType parentType = e.Parent;
        if (parentType == null) {
          break;
        }
        var parent = new Element(parentType, _defaultAttributes);
        //			System.err.println("%% Ascending from " + e.name() + " to " + parent.name());
        parent.Next = e;
        e = parent;
      }
      if (sp == null) {
        return; // don't know what to do
      }
      while (_stack != sp) {
        if (_stack == null || _stack.Next == null || _stack.Next.Next == null) {
          break;
        }
        RestartablyPop();
      }
      while (e != null) {
        Element nexte = e.Next;
        if (!e.Name.Equals("<pcdata>")) {
          Push(e);
        }
        e = nexte;
        Restart(e);
      }
      _newElement = null;
    }

    /// <summary>
    ///   Return the argument as a valid XML name
    ///   This no longer lowercases the result: we depend on Schema to
    ///   canonicalize case.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private string MakeName(char[] buff, int offset, int length) {
      var dst = new StringBuilder(length + 2);
      bool seenColon = false;
      bool start = true;
      //		string src = new string(buff, offset, length); // DEBUG
      for (; length-- > 0; offset++) {
        char ch = buff[offset];
        if (char.IsLetter(ch) || ch == '_') {
          start = false;
          dst.Append(ch);
        } else if (char.IsDigit(ch) || ch == '-' || ch == '.') {
          if (start) {
            dst.Append('_');
          }
          start = false;
          dst.Append(ch);
        } else if (ch == ':' && !seenColon) {
          seenColon = true;
          if (start) {
            dst.Append('_');
          }
          start = true;
          dst.Append(_translateColons ? '_' : ch);
        }
      }
      int dstLength = dst.Length;
      if (dstLength == 0 || dst[dstLength - 1] == ':') {
        dst.Append('_');
      }
      //		System.err.println("Made name \"" + dst + "\" from \"" + src + "\"");
      return string.Intern(dst.ToString());
    }

    private class AutoDetectorDelegate : IAutoDetector {
      private readonly Func<Stream, StreamReader> _delegate;

      public AutoDetectorDelegate(Func<Stream, StreamReader> @delegate) {
        _delegate = @delegate;
      }

      public TextReader AutoDetectingReader(Stream stream) {
        return _delegate(stream);
      }
    }
  }
}
