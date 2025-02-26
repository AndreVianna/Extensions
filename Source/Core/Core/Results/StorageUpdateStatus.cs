namespace DotNetToolbox.Results;

public enum StorageUpdateStatus {
    Pending,
    InvalidInput,
    RecordNotFound,
    MultipleRecordsFound,
    DataConflict,
    Successful,
}
