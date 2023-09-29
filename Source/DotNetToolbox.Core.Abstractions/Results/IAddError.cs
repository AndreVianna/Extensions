﻿using System.Validation;

namespace System.Results;

public interface IAddError<TSelf> :
    IAdditionOperators<TSelf, IValidationError, TSelf>
    where TSelf : IAddError<TSelf>
{
}