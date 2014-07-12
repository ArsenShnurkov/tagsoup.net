namespace TagSoup.Net {
  using System.IO;

  /// <summary>
  ///     An interface allowing Parser to invoke scanners.
  /// </summary>
  public interface IScanner {
    /// <summary>
    ///     Invoke a scanner.
    /// </summary>
    /// <param name="br">
    ///     A source of characters to scan
    /// </param>
    /// <param name="handler">
    ///     A ScanHandler to report events to
    /// </param>
    void Scan(TextReader br, IScanHandler handler);

    /// <summary>
    ///     Reset the embedded locator.
    /// </summary>
    /// <param name="publicid">
    ///     The publicid of the source
    /// </param>
    /// <param name="systemid">
    ///     The systemid of the source
    /// </param>
    void ResetDocumentLocator(string publicid, string systemid);

    /// <summary>
    ///     Signal to the scanner to start CDATA content mode.
    /// </summary>
    void StartCDATA();
  }
}
