﻿namespace System.Results;

public interface ICreateValidationResults<out TSelf> {
    static abstract TSelf Failure([StringSyntax(CompositeFormat)] string message, params object?[] args);
    static abstract TSelf Failure(IValidationResult result);
    static abstract TSelf Failure(IEnumerable<ValidationError> errors);
    static abstract TSelf Failure(ValidationError error);
    static abstract TSelf Success();
}