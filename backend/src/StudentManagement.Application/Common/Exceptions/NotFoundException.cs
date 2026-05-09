namespace StudentManagement.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string stMessage) : base(stMessage)
    {
    }

    public NotFoundException(string stEntity, object objKey)
        : base($"{stEntity} with key '{objKey}' was not found.")
    {
    }
}
