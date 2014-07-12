namespace TagSoup.Net {
  using System;
  using System.Text;

  using Sax.Net.Helpers;

  /// <summary>
  ///     This class represents an element type in the schema.
  ///     An element type has a name, a content model vector, a member-of vector,
  ///     a flags vector, default attributes, and a schema to which it belongs.
  /// </summary>
  /// <seealso cref="Schema" />
  public class ElementType {
    private readonly Attributes _atts; // default attributes
    private readonly string _localName; // element type local name
    private readonly string _name; // element type name (Qname)
    private readonly string _namespace; // element type namespace name
    private readonly Schema _schema; // schema to which this belongs

    /// <summary>
    ///     Construct an ElementType:
    ///     but it's better to use Schema.element() instead.
    ///     The content model, member-of, and flags vectors are specified as ints.
    /// </summary>
    /// <param name="name">
    ///     The element type name
    /// </param>
    /// <param name="model">
    ///     ORed-together bits representing the content models
    ///     allowed in the content of this element type
    /// </param>
    /// <param name="memberOf">
    ///     ORed-together bits representing the content models
    ///     to which this element type belongs
    /// </param>
    /// <param name="flags">
    ///     ORed-together bits representing the flags associated
    ///     with this element type
    /// </param>
    /// <param name="schema">
    ///     The schema with which this element type will be
    ///     associated
    /// </param>
    public ElementType(string name, int model, int memberOf, int flags, Schema schema) {
      _name = name;
      Model = model;
      MemberOf = memberOf;
      Flags = flags;
      _atts = new Attributes();
      _schema = schema;
      _namespace = GetNamespace(name, false);
      _localName = GetLocalName(name);
    }

    /// <summary>
    ///     Gets the name of this element type.
    /// </summary>
    public string Name {
      get { return _name; }
    }

    /// <summary>
    ///     Gets the namespace name of this element type.
    /// </summary>
    public string Namespace {
      get { return _namespace; }
    }

    /// <summary>
    ///     Gets the local name of this element type.
    /// </summary>
    public string LocalName {
      get { return _localName; }
    }

    /// <summary>
    ///     Gets or sets the content models of this element type as a vector of bits
    /// </summary>
    public int Model { get; set; }

    /// <summary>
    ///     Gets or sets the content models to which this element type belongs as a vector of bits
    /// </summary>
    public int MemberOf { get; set; }

    /// <summary>
    ///     Gets or sets the flags associated with this element type as a vector of bits
    /// </summary>
    public int Flags { get; set; }

    /// <summary>
    ///     Returns the default attributes associated with this element type.
    ///     Attributes of type CDATA that don't have default values are
    ///     typically not included.  Other attributes without default values
    ///     have an internal value of <c>null</c>.
    ///     The return value is an Attributes to allow the caller to mutate
    ///     the attributes.
    /// </summary>
    public Attributes Attributes {
      get { return _atts; }
    }

    /// <summary>
    ///     Gets or sets the parent element type of this element type.
    /// </summary>
    public ElementType Parent { get; set; }

    /// <summary>
    ///     Gets the schema which this element type is associated with.
    /// </summary>
    public Schema Schema {
      get { return _schema; }
    }

    /// <summary>
    ///     Return a namespace name from a Qname.
    ///     The attribute flag tells us whether to return an empty namespace
    ///     name if there is no prefix, or use the schema default instead.
    /// </summary>
    /// <param name="name">
    ///     The Qname
    /// </param>
    /// <param name="attribute">
    ///     True if name is an attribute name
    /// </param>
    /// <returns>
    ///     The namespace name
    /// </returns>
    public string GetNamespace(string name, bool attribute) {
      int colon = name.IndexOf(':');
      if (colon == -1) {
        return attribute ? "" : _schema.Uri;
      }
      string prefix = name.Substring(0, colon);
      if (prefix.Equals("xml")) {
        return "http://www.w3.org/XML/1998/namespace";
      }
      return string.Intern("urn:x-prefix:" + prefix);
    }

    /// <summary>
    ///     Return a local name from a Qname.
    /// </summary>
    /// <param name="name">
    ///     The Qname
    /// </param>
    /// <returns>
    ///     The local name
    /// </returns>
    public string GetLocalName(string name) {
      int colon = name.IndexOf(':');
      if (colon == -1) {
        return name;
      }
      return string.Intern(name.Substring(colon + 1));
    }

    ////
    /// <summary>
    ///     Returns <c>true</c> if this element type can contain another element type.
    ///     That is, if any of the models in this element's model vector
    ///     match any of the models in the other element type's member-of
    ///     vector.
    /// </summary>
    /// <param name="other">
    ///     The other element type
    /// </param>
    public bool CanContain(ElementType other) {
      return (Model & other.MemberOf) != 0;
    }

    ////
    /// <summary>
    ///     Sets an attribute and its value into an Attributes object.
    ///     Attempts to set a namespace declaration are ignored.
    /// </summary>
    /// <param name="atts">
    ///     The AttributesImpl object
    /// </param>
    /// <param name="name">
    ///     The name (Qname) of the attribute
    /// </param>
    /// <param name="type">
    ///     The type of the attribute
    /// </param>
    /// <param name="value">
    ///     The value of the attribute
    /// </param>
    public void SetAttribute(Attributes atts, string name, string type, string value) {
      if (name.Equals("xmlns") || name.StartsWith("xmlns:")) {
        return;
      }

      string ns = GetNamespace(name, true);
      string localName = GetLocalName(name);
      int i = atts.GetIndex(name);
      if (i == -1) {
        name = string.Intern(name);
        if (type == null) {
          type = "CDATA";
        }
        if (!type.Equals("CDATA")) {
          value = Normalize(value);
        }
        atts.AddAttribute(ns, localName, name, type, value);
      } else {
        if (type == null) {
          type = atts.GetType(i);
        }
        if (!type.Equals("CDATA")) {
          value = Normalize(value);
        }
        atts.SetAttribute(i, ns, localName, name, type, value);
      }
    }

    /// <summary>
    ///     Normalize an attribute value (ID-style).
    ///     CDATA-style attribute normalization is already done.
    /// </summary>
    /// <param name="value">
    ///     The value to normalize
    /// </param>
    /// <returns>
    /// </returns>
    public static string Normalize(string value) {
      if (value == null) {
        return null;
      }
      value = value.Trim();
      if (value.IndexOf("  ", StringComparison.Ordinal) == -1) {
        return value;
      }
      bool space = false;
      var b = new StringBuilder(value.Length);
      foreach (char v in value) {
        if (v == ' ') {
          if (!space) {
            b.Append(v);
          }
          space = true;
        } else {
          b.Append(v);
          space = false;
        }
      }
      return b.ToString();
    }

    /// <summary>
    ///     Sets an attribute and its value into this element type.
    /// </summary>
    /// <param name="name">
    ///     The name of the attribute
    /// </param>
    /// <param name="type">
    ///     The type of the attribute
    /// </param>
    /// <param name="value">
    ///     The value of the attribute
    /// </param>
    public void SetAttribute(string name, string type, string value) {
      SetAttribute(_atts, name, type, value);
    }
  }
}
