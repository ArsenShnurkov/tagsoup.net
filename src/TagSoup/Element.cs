namespace TagSoup.Net {
  using Sax.Net.Helpers;

  /// <summary>
  ///     The internal representation of an actual element (not an element type).
  ///     An Element has an element type, attributes, and a successor Element
  ///     for use in constructing stacks and queues of Elements.
  /// </summary>
  /// <seealso cref="ElementType" />
  /// <seealso cref="Sax.Net.Helpers.Attributes" />
  public class Element {
    private readonly Attributes _atts; // attributes of element
    private readonly ElementType _type; // type of element
    private bool _preclosed; // this element has been preclosed

    /// <summary>
    ///     Return an Element from a specified ElementType.
    /// </summary>
    /// <param name="type">
    ///     The element type of the newly constructed element
    /// </param>
    /// <param name="defaultAttributes">
    ///     True if default attributes are wanted
    /// </param>
    public Element(ElementType type, bool defaultAttributes) {
      _type = type;
      if (defaultAttributes) {
        _atts = new Attributes(type.Attributes);
      } else {
        _atts = new Attributes();
      }
      Next = null;
      _preclosed = false;
    }

    /// <summary>
    ///     Gets the element type.
    /// </summary>
    public ElementType Type {
      get { return _type; }
    }

    /// <summary>
    ///     Gets the attributes as an Attributes object.
    ///     Returning an Attributes makes the attributes mutable.
    /// </summary>
    /// <seealso cref="Attributes" />
    public Attributes Attributes {
      get { return _atts; }
    }

    /// <summary>
    ///     Gets or sets the next element in an element stack or queue.
    /// </summary>
    public Element Next { get; set; }

    /// <summary>
    ///     Gets the name of the element's type.
    /// </summary>
    public string Name {
      get { return _type.Name; }
    }

    /// <summary>
    ///     Gets the namespace name of the element's type.
    /// </summary>
    public string Namespace {
      get { return _type.Namespace; }
    }

    /// <summary>
    ///     Gets the local name of the element's type.
    /// </summary>
    public string LocalName {
      get { return _type.LocalName; }
    }

    /// <summary>
    ///     Gets the content model vector of the element's type.
    /// </summary>
    public int Model {
      get { return _type.Model; }
    }

    /// <summary>
    ///     Gets the member-of vector of the element's type.
    /// </summary>
    public int MemberOf {
      get { return _type.MemberOf; }
    }

    /// <summary>
    ///     Gets the flags vector of the element's type.
    /// </summary>
    public int Flags {
      get { return _type.Flags; }
    }

    /// <summary>
    ///     Gets the parent element type of the element's type.
    /// </summary>
    public ElementType Parent {
      get { return _type.Parent; }
    }

    /// <summary>
    ///     Return true if this element has been preclosed.
    /// </summary>
    public bool IsPreclosed {
      get { return _preclosed; }
    }

    /// <summary>
    ///     Return true if the type of this element can contain the type of
    ///     another element.
    ///     Convenience method.
    /// </summary>
    /// <param name="other">
    ///     The other element
    /// </param>
    public bool CanContain(Element other) {
      return _type.CanContain(other._type);
    }

    /// <summary>
    ///     Set an attribute and its value into this element.
    /// </summary>
    /// <param name="name">
    ///     The attribute name (Qname)
    /// </param>
    /// <param name="type">
    ///     The attribute type
    /// </param>
    /// <param name="value">
    ///     The attribute value
    /// </param>
    public void SetAttribute(string name, string type, string value) {
      _type.SetAttribute(_atts, name, type, value);
    }

    /// <summary>
    ///     Make this element anonymous.
    ///     Remove any <tt>id</tt> or <tt>name</tt> attribute present
    ///     in the element's attributes.
    /// </summary>
    public void Anonymize() {
      for (int i = _atts.Length - 1; i >= 0; i--) {
        if (_atts.GetType(i).Equals("ID") || _atts.GetQName(i).Equals("name")) {
          _atts.RemoveAttribute(i);
        }
      }
    }

    /// <summary>
    ///     Clean the attributes of this element.
    ///     Attributes with null name (the name was ill-formed)
    ///     or null value (the attribute was present in the element type but
    ///     not in this actual element) are removed.
    /// </summary>
    public void Clean() {
      for (int i = _atts.Length - 1; i >= 0; i--) {
        string name = _atts.GetLocalName(i);
        if (_atts.GetValue(i) == null || string.IsNullOrEmpty(name)) {
          _atts.RemoveAttribute(i);
        }
      }
    }

    /// <summary>
    ///     Force this element to preclosed status, meaning that an end-tag has
    ///     been seen but the element cannot yet be closed for structural reasons.
    /// </summary>
    public void Preclose() {
      _preclosed = true;
    }
  }
}
