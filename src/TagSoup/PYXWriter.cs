// PYX Writer
// FIXME: does not do escapes in attribute values
// FIXME: outputs entities as bare '&' character

namespace TagSoup.Net {
  using System.IO;

  using Sax.Net;
  using Sax.Net.Ext;

  /// <summary>
  ///     A ContentHandler that generates PYX format instead of XML.
  ///     Primarily useful for debugging.
  /// </summary>
  public class PYXWriter : IScanHandler, IContentHandler, ILexicalHandler {
    private static char[] dummy = new char[1];
    private readonly TextWriter _writer; // where we Write to
    private string _attrName; // saved attribute name

    public PYXWriter(TextWriter w) {
      _writer = w;
    }

    public void Characters(char[] buff, int offset, int length) {
      PCDATA(buff, offset, length);
    }

    public void EndDocument() {
      _writer.Close();
    }

    public void EndElement(string uri, string localname, string qname) {
      if (qname.Length == 0) {
        qname = localname;
      }
      _writer.Write(')');
      _writer.WriteLine(qname);
    }

    public void EndPrefixMapping(string prefix) {
    }

    public void IgnorableWhitespace(char[] buff, int offset, int length) {
      Characters(buff, offset, length);
    }

    public void ProcessingInstruction(string target, string data) {
      _writer.Write('?');
      _writer.Write(target);
      _writer.Write(' ');
      _writer.WriteLine(data);
    }

    public void SetDocumentLocator(ILocator locator) {
    }

    public void SkippedEntity(string name) {
    }

    public void StartDocument() {
    }

    public void StartElement(string uri, string localname, string qname, IAttributes atts) {
      if (qname.Length == 0) {
        qname = localname;
      }
      _writer.Write('(');
      _writer.WriteLine(qname);
      int length = atts.Length;
      for (int i = 0; i < length; i++) {
        qname = atts.GetQName(i);
        if (qname.Length == 0) {
          qname = atts.GetLocalName(i);
        }
        _writer.Write('A');
        //			theWriter.Write(atts.getType(i));	// DEBUG
        _writer.Write(qname);
        _writer.Write(' ');
        _writer.WriteLine(atts.GetValue(i));
      }
    }

    public void StartPrefixMapping(string prefix, string uri) {
    }

    public void Comment(char[] ch, int start, int length) {
      Cmnt(ch, start, length);
    }

    public void EndCDATA() {
    }

    public void EndDTD() {
    }

    public void EndEntity(string name) {
    }

    public void StartCDATA() {
    }

    public void StartDTD(string name, string publicId, string systemId) {
    }

    public void StartEntity(string name) {
    }

    public void Adup(char[] buff, int offset, int length) {
      _writer.WriteLine(_attrName);
      _attrName = null;
    }

    public void Aname(char[] buff, int offset, int length) {
      _writer.Write('A');
      _writer.Write(buff, offset, length);
      _writer.Write(' ');
      _attrName = new string(buff, offset, length);
    }

    public void Aval(char[] buff, int offset, int length) {
      _writer.Write(buff, offset, length);
      _writer.WriteLine();
      _attrName = null;
    }

    public void Cmnt(char[] buff, int offset, int length) {
      //		theWriter.Write('!');
      //		theWriter.Write(buff, offset, length);
      //		theWriter.WriteLine();
    }

    public void Entity(char[] buff, int offset, int length) {
    }

    public int GetEntity() {
      return 0;
    }

    public void EOF(char[] buff, int offset, int length) {
      _writer.Close();
    }

    public void ETag(char[] buff, int offset, int length) {
      _writer.Write(')');
      _writer.Write(buff, offset, length);
      _writer.WriteLine();
    }

    public void GI(char[] buff, int offset, int length) {
      _writer.Write('(');
      _writer.Write(buff, offset, length);
      _writer.WriteLine();
    }

    public void CDSect(char[] buff, int offset, int length) {
      PCDATA(buff, offset, length);
    }

    public void PCDATA(char[] buff, int offset, int length) {
      if (length == 0) {
        return; // nothing to do
      }
      bool inProgress = false;
      length += offset;
      for (int i = offset; i < length; i++) {
        if (buff[i] == '\n') {
          if (inProgress) {
            _writer.WriteLine();
          }
          _writer.WriteLine("-\\n");
          inProgress = false;
        } else {
          if (!inProgress) {
            _writer.Write('-');
          }
          switch (buff[i]) {
            case '\t':
              _writer.Write("\\t");
              break;
            case '\\':
              _writer.Write("\\\\");
              break;
            default:
              _writer.Write(buff[i]);
              break;
          }
          inProgress = true;
        }
      }
      if (inProgress) {
        _writer.WriteLine();
      }
    }

    public void PITarget(char[] buff, int offset, int length) {
      _writer.Write('?');
      _writer.Write(buff, offset, length);
      _writer.Write(' ');
    }

    public void PI(char[] buff, int offset, int length) {
      _writer.Write(buff, offset, length);
      _writer.WriteLine();
    }

    public void STagc(char[] buff, int offset, int length) {
      //		theWriter.WriteLine("!");			// FIXME
    }

    public void STage(char[] buff, int offset, int length) {
      _writer.WriteLine("!"); // FIXME
    }

    public void Decl(char[] buff, int offset, int length) {
    }
  }
}
