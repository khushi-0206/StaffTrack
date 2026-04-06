namespace TimeSheetService.Application.Common.Exceptions;

public class AppException : Exception
{
    public AppException(string message, int statusCode = 400) : base(message) => StatusCode = statusCode;
    public int StatusCode { get; }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message = "Forbidden") : base(message, 403) { }
}
