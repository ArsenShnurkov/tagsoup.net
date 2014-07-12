namespace TagSoup.Net {
  using System.IO;

  /// <summary>
  ///     Classes which accept an Stream and provide a Reader which figures
  ///     out the encoding of the Stream and reads characters from it should
  ///     conform to this interface.
  /// </summary>
  /// <seealso cref="Stream" />
  /// <seealso cref="TextReader" />
  public interface IAutoDetector {
    /// <summary>
    ///     Given an InputStream, return a suitable Reader that understands
    ///     the presumed character encoding of that InputStream.
    ///     If bytes are consumed from the InputStream in the process, they
    ///     <i>must</i> be pushed back onto the InputStream so that they can be
    ///     reinterpreted as characters.
    /// </summary>
    /// <param name="stream">
    ///     The Stream
    /// </param>
    /// <returns>A TextReader that reads from the Stream</returns>
    TextReader AutoDetectingReader(Stream stream);
  }
}
