namespace DotNetToolbox.Results;

public enum HttpRequestState : ushort {
    Ok = HttpStatusCode.OK, // 200
    Created = HttpStatusCode.Created, // 201

    BadRequest = HttpStatusCode.BadRequest, // 400
    Unauthorized = HttpStatusCode.Unauthorized, // 401
    NotFound = HttpStatusCode.NotFound, // 404
    Conflict = HttpStatusCode.Conflict, // 409

    ServerError = HttpStatusCode.InternalServerError, // 500
}
