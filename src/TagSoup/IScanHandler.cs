namespace TagSoup.Net {
  /// <summary>
  ///     An interface that Scanners use to report events in the input stream.
  /// </summary>
  public interface IScanHandler {
    /// <summary>
    ///     Reports an attribute name without a value.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Adup(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports an attribute name; a value will follow.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Aname(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports an attribute value.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Aval(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports the content of a CDATA section (not a CDATA element)
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void CDSect(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports a &lt;!....&gt; declaration - typically a DOCTYPE
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Decl(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports an entity reference or character reference.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Entity(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports EOF.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void EOF(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports an end-tag.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void ETag(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports the general identifier (element type name) of a start-tag.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void GI(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports character content.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void PCDATA(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports the data part of a processing instruction.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void PI(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports the target part of a processing instruction.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void PITarget(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports the close of a start-tag.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void STagc(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports the close of an empty-tag.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void STage(char[] buff, int offset, int length);

    /// <summary>
    ///     Reports a comment.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Cmnt(char[] buff, int offset, int length);

    /// <summary>
    ///     Returns the value of the last entity or character reference reported.
    /// </summary>
    /// <returns>The value of the last entity or character reference reported.</returns>
    int GetEntity();
  }
}
