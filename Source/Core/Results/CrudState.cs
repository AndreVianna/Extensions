namespace DotNetToolbox.Results;

public enum CrudState : byte {
    Success = 0, // The operation was successful.
    Invalid = 1, // The request validation failed.
    NotFound = 2, // The requested resource was not found.
    Conflict = 3, // A conflict has occured blocking the operation.
}
