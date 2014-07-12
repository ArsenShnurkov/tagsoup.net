namespace TagSoup.Net {
  using System.IO;

  /// <summary>
  ///     A Scanner that accepts PYX format instead of HTML.
  ///     Useful primarily for debugging.
  /// </summary>
  public class PYXScanner : IScanner {
    public void ResetDocumentLocator(string publicid, string systemid) {
      // Need this method for interface compatibility, but note
      // that PyxScanner does not implement Locator.
    }

    public void Scan(TextReader br, IScanHandler h) {
      string s;
      char[] buff = null;
      bool instag = false;
      while ((s = br.ReadLine()) != null) {
        int size = s.Length;
        buff = s.ToCharArray(0, size);
        if (buff.Length < size) {
          buff = new char[size];
        }
        switch (buff[0]) {
          case '(':
            if (instag) {
              h.STagc(buff, 0, 0);
              instag = false;
            }
            h.GI(buff, 1, size - 1);
            instag = true;
            break;
          case ')':
            if (instag) {
              h.STagc(buff, 0, 0);
              instag = false;
            }
            h.ETag(buff, 1, size - 1);
            break;
          case '?':
            if (instag) {
              h.STagc(buff, 0, 0);
              instag = false;
            }
            h.PI(buff, 1, size - 1);
            break;
          case 'A':
            int sp = s.IndexOf(' ');
            h.Aname(buff, 1, sp - 1);
            h.Aval(buff, sp + 1, size - sp - 1);
            break;
          case '-':
            if (instag) {
              h.STagc(buff, 0, 0);
              instag = false;
            }
            if (s.Equals("-\\n")) {
              buff[0] = '\n';
              h.PCDATA(buff, 0, 1);
            } else {
              // FIXME:
              // Does not decode \t and \\ in input
              h.PCDATA(buff, 1, size - 1);
            }
            break;
          case 'E':
            if (instag) {
              h.STagc(buff, 0, 0);
              instag = false;
            }
            h.Entity(buff, 1, size - 1);
            break;
          default:
            //				System.err.print("Gotcha ");
            //				System.err.print(s);
            //				System.err.print('\n');
            break;
        }
      }
      h.EOF(buff, 0, 0);
    }

    public void StartCDATA() {
    }

    //public static void main(string[] argv)  {
    //  IScanner s = new PYXScanner();
    //  TextReader r = new StreamReader(System.Console.OpenStandardInput(), Encoding.UTF8);
    //  TextWriter w = new StreamWriter(System.Console.OpenStandardOutput(), Encoding.UTF8));
    //  s.Scan(r, new PYXWriter(w));
    //  }
  }
}
