using System.Net;
using System.Text.Json.Serialization;

namespace Application.Common.Utils;

public record Result<T>(
    bool IsSuccess,
    [property: JsonIgnore]
    HttpStatusCode StatusCode,
    T? Value,
    string[] Errors);