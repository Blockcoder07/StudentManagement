namespace StudentManagement.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string stMessage) : base(stMessage)
    {
    }
}
