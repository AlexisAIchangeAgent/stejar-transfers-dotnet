namespace StejarTransfers.Domain;

/// <summary>The transfer cannot be priced by the current rules.</summary>
public sealed class UnsupportedTransferException(string message) : Exception(message);
