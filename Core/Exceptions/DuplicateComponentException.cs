namespace MystiickCore.Exceptions;

public class DuplicateComponentException : Exception
{
    public DuplicateComponentException() : base() { }
    public DuplicateComponentException(string message) : base(message) { }
}
