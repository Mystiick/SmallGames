namespace MystiickCore.Exceptions;

public class DuplicateEngineProcessingOrderException : Exception
{
    public DuplicateEngineProcessingOrderException() : base() { }
    public DuplicateEngineProcessingOrderException(int processingOrder) : base($"Unable to add another engine with the same Processing Order of {processingOrder}") { }
    public DuplicateEngineProcessingOrderException(string message) : base(message) { }
}
