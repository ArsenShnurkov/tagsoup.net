namespace TagSoup.Net {
  using System;
  using System.Collections;

  /// <summary>
  ///     Abstract class representing a TSSL schema.
  ///     Actual TSSL schemas are compiled into concrete subclasses of this class.
  /// </summary>
  public abstract class Schema {
    public const int M_ANY = -1;//0xFFFFFFFF;
    public const int M_EMPTY = 0;
    public const int M_PCDATA = 1 << 30;
    public const int M_ROOT = 1 << 31;

    public const int F_RESTART = 1;
    public const int F_CDATA = 2;
    public const int F_NOFORCE = 4;

    private readonly Hashtable _elementTypes = new Hashtable(); // string -> ElementType
    private readonly Hashtable _entities = new Hashtable(); // string -> Character

    private string _prefix = "";
    private ElementType _root;
    private string _uri = "";

    /// <summary>
    ///     Gets or sets the URI (namespace name) of this schema.
    /// </summary>
    public string Uri {
      get { return _uri; }
      set { _uri = value; }
    }

    /// <summary>
    ///     Gets ot sets the prefix of this schema.
    /// </summary>
    public string Prefix {
      get { return _prefix; }
      set { _prefix = value; }
    }

    /// <summary>
    ///     Get the root element of this schema
    /// </summary>
    public ElementType RootElementType {
      get { return _root; }
    }

    /// <summary>
    ///     Add or replace an element type for this schema.
    /// </summary>
    /// <param name="name">
    ///     Name (Qname) of the element
    /// </param>
    /// <param name="model">
    ///     Models of the element's content as a vector of bits
    /// </param>
    /// <param name="memberOf">
    ///     Models the element is a member of as a vector of bits
    /// </param>
    /// <param name="flags">
    ///     Flags for the element
    /// </param>
    public void ElementType(string name, int model, int memberOf, int flags) {
      var e = new ElementType(name, model, memberOf, flags, this);
      _elementTypes[name.ToLower()] = e;
      if (memberOf == M_ROOT) {
        _root = e;
      }
    }

    /// <summary>
    ///     Add or replace a default attribute for an element type in this schema.
    /// </summary>
    /// <param name="elemName">
    ///     Name (Qname) of the element type
    /// </param>
    /// <param name="attrName">
    ///     Name (Qname) of the attribute
    /// </param>
    /// <param name="type">
    ///     Type of the attribute
    /// </param>
    /// <param name="value">
    ///     Default value of the attribute; null if no default
    /// </param>
    public void Attribute(string elemName, string attrName, string type, string value) {
      ElementType e = GetElementType(elemName);
      if (e == null) {
        throw new Exception("Attribute " + attrName + " specified for unknown element type " + elemName);
      }
      e.SetAttribute(attrName, type, value);
    }

    /// <summary>
    ///     Specify natural parent of an element in this schema.
    /// </summary>
    /// <param name="name">
    ///     Name of the child element
    /// </param>
    /// <param name="parentName">
    ///     Name of the parent element
    /// </param>
    public void Parent(string name, string parentName) {
      ElementType child = GetElementType(name);
      ElementType parent = GetElementType(parentName);
      if (child == null) {
        throw new Exception("No child " + name + " for parent " + parentName);
      }
      if (parent == null) {
        throw new Exception("No parent " + parentName + " for child " + name);
      }
      child.Parent = parent;
    }

    /// <summary>
    ///     Add to or replace a character entity in this schema.
    /// </summary>
    /// <param name="name">
    ///     Name of the entity
    /// </param>
    /// <param name="value">
    ///     Value of the entity
    /// </param>
    public void Entity(string name, int value) {
      _entities[name] = value;
    }

    /// <summary>
    ///     Get an ElementType by name.
    /// </summary>
    /// <param name="name">
    ///     Name (Qname) of the element type
    /// </param>
    /// <returns>
    ///     The corresponding ElementType
    /// </returns>
    public ElementType GetElementType(string name) {
      return (ElementType)(_elementTypes[name.ToLower()]);
    }

    /// <summary>
    ///     Get an entity value by name.
    /// </summary>
    /// <param name="name">
    ///     Name of the entity
    /// </param>
    /// <returns>
    ///     The corresponding character, or 0 if none
    /// </returns>
    public int GetEntity(string name) {
      //		System.err.println("%% Looking up entity " + name);
      if (_entities.ContainsKey(name)) {
        return (int)_entities[name];
      }
      return 0;
    }
  }
}
