using System.Net;

namespace Application.Common.Utils;

public static class ResultBuilder 
{
    public static Result<T> SuccessResult<T>(T data) => new (true, HttpStatusCode.OK, data, [string.Empty]);
    
    public static Result<T> CreatedResult<T>(T data) => new (true, HttpStatusCode.Created, data, [string.Empty]);
    
    public static Result<T> NoContentResult<T>() => new (true, HttpStatusCode.NoContent, default, [string.Empty]);
    
    public static Result<T> UnauthorizedResult<T>(string message) => new (false, HttpStatusCode.Unauthorized, default, [message]);
    
    public static Result<T> NotFoundResult<T>(string message) => new (false, HttpStatusCode.NotFound, default, [message]);
    
    public static Result<T> ConflictResult<T>(string message) => new (false, HttpStatusCode.Conflict, default, [message]);
    
    public static Result<T> InternalServerErrorResult<T>(string message) => new (false, HttpStatusCode.InternalServerError, default, [message]);
}