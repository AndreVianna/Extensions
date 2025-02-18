## Core (DotNetToolbox.Core)

### Introduction
DotNetToolbox.Core is a versatile C# library for .NET 8, designed to enhance development by providing a range of utilities and patterns. It simplifies complex tasks and improves code testability.

### Table of Contents
1. [Installation](#installation)
2. [Dependencies](#dependencies)
3. [Result Pattern](#result-pattern)
4. [System Utilities](#system-utilities)
5. [Pagination Utilities](#pagination-utilities)
6. [Singleton and Options Patterns](#singleton-and-options-patterns)
7. [Extension Classes](#extension-classes)
8. [Other Utilities](#other-utilities)

### Installation
```shell
PM> Install-Package DotNetToolbox.Core
```

### Dependencies
- .NET 8

### Result Pattern

The exploration of the `Result` class and its specialized forms, including `CrudResult`, `HttpResult`, and `SignInResult`, underscores the pattern's versatility and utility in diverse application domains. By providing a structured approach to operation outcome management, these patterns enhance the clarity, maintainability, and reliability of software, facilitating robust error handling and streamlined communication of operation results. 

### ValidationError and ValidationErrors

The `ValidationError` class encapsulates information about errors that occur during validation processes. It is designed as a sealed record for immutability and type safety, with a default error message indicating invalid values. This class supports initializing with custom messages and an optional source, enhancing error specificity and traceability.

```csharp
var error = new ValidationError("Custom error message", "FieldName");
```

An implicit conversion from string to `ValidationError` is provided, allowing for straightforward error creation:

```csharp
ValidationError error = "This is an error message";
```

`ValidationErrors` acts as a collection for `ValidationError` objects, implementing standard list and collection interfaces for easy manipulation. It supports initializing from various sources, including single errors, arrays, lists, and hash sets, each ensuring uniqueness of error entries:

```csharp
ValidationErrors errors = new ValidationError[] {
    "Error 1",
    new ValidationError("Error 2"),
    "Error 3"
};
```

This collection facilitates aggregating and handling multiple validation errors in a coherent and structured manner, streamlining error management in validation scenarios:

```csharp
if (errors.Any()) {
    foreach (var err in errors) {
        Console.WriteLine(err);
    }
}
```

These utilities serve as foundational elements within the Result Pattern, enabling detailed and flexible error reporting in validation operations.

### Result Pattern Implementation

The Result pattern is a robust framework feature designed to encapsulate the outcome of operations, offering a unified approach to error handling and validation. It provides a clear and consistent method for functions to return success or failure information, along with any relevant data or errors.

#### IResult and Result Usage

The `IResult` interface is the foundation of the Result pattern, defining a contract for operation outcomes. Implementing classes can specify more details, such as success status, error messages, or returned data. The `Result` class, deriving from `ResultBase`, offers a concrete implementation that can be used directly in applications:

```csharp
public Result DoSomething() {
    try {
        // Operation logic
        return Result.Success();
    } catch (Exception ex) {
        return Result.Failure(ex.Message);
    }
}
```

#### Integrating Validation with IValidatable

The `IValidatable` interface allows for the integration of custom validation logic into models or data transfer objects, returning a `Result` to indicate the validation outcome. This pattern ensures that validation logic can be encapsulated within the objects themselves, promoting a clean and maintainable codebase:

```csharp
public class MyModel : IValidatable {
    public string Property { get; set; }

    public Result Validate() {
        if (string.IsNullOrEmpty(Property)) {
            return Result.Failure("Property cannot be null or empty.");
        }
        return Result.Success();
    }
}
```

#### Handling Validation Errors

When validation fails, a `ValidationException` can be thrown, leveraging the `Result` pattern for consistent error handling across the application. This exception specifically addresses validation failures, allowing for targeted catch blocks and error processing:

```csharp
try {
    var model = new MyModel();
    var result = model.Validate();
    if (!result.IsSuccess) {
        throw new ValidationException(result.Errors);
    }
} catch (ValidationException ex) {
    // Handle validation errors
}
```

#### Result Types and Error Management

The `ResultType` enum and `ResultBase` class support extending the Result pattern with custom outcomes and sophisticated error management strategies. Developers can define additional result types beyond the basic success and failure, accommodating complex business logic and operation outcomes:

```csharp
public enum MyCustomResultType : int {
    CustomSuccess = 1,
    CustomFailure = -1
}

public class MyCustomResult : ResultBase {
    // Custom implementation
}
```

### CrudResult Pattern for CRUD Operations

The `CrudResult` class extends the foundational `Result` pattern specifically for CRUD (Create, Read, Update, Delete) operations, providing a clear and expressive way to communicate the outcomes of these common database or resource manipulation tasks.

#### Understanding CrudResult Types

At the heart of the `CrudResult` design is the `CrudResultType` enum, which categorizes the outcomes of CRUD operations into statuses like `Success`, `NotFound`, and `Conflict`. This detailed classification aids in handling the varied results of CRUD operations with precision:

```csharp
public enum CrudResultType {
    Invalid, // The request validation failed.
    NotFound, // The requested resource was not found.
    Conflict, // A conflict has occured blocking the operation.
    Success, // The operation was successful.
    Error, // An internal error has occurred.
}
```

#### Using CrudResult in Operations

`CrudResult` simplifies returning detailed operation outcomes. For instance, when updating a resource, `CrudResult` can distinctly indicate whether the operation was successful, the target resource was not found, or a conflict occurred:

```csharp
public CrudResult UpdateResource(Resource resource) {
    if (!ResourceExists(resource.Id)) {
        return CrudResult.NotFound();
    }
    if (ResourceConflictExists(resource)) {
        return CrudResult.Conflict();
    }
    // Update logic here
    return CrudResult.Success();
}
```

#### Leveraging Generic CrudResult

For operations that return a value, such as retrieving a resource, `CrudResult<T>` comes into play, allowing the inclusion of the resource itself in the success case:

```csharp
public CrudResult<Resource> GetResource(int id) {
    var resource = FindResourceById(id);
    if (resource == null) {
        return CrudResult<Resource>.NotFound();
    }
    return CrudResult<Resource>.Success(resource);
}
```

#### Benefits of CrudResult

The `CrudResult` class and its generic counterpart `CrudResult<T>` provide a robust, type-safe way to handle the outcomes of CRUD operations, enhancing code clarity and maintainability. By clearly communicating operation results, developers can implement more reliable and understandable error handling and response logic in applications dealing with data manipulation.


### HttpResult for Web API Responses

The `HttpResult` class is a specialized adaptation of the Result pattern, tailored specifically for handling the outcomes of web API operations. It provides a seamless way to align the internal operation results with the standardized semantics of HTTP responses.

#### Overview of HttpResult

`HttpResult` facilitates the expression of HTTP response statuses through its methods, which correspond to common HTTP status codes. This design allows developers to directly return HTTP responses from API operations, enhancing clarity and consistency:

```csharp
public enum HttpResultType {
    Ok = HttpStatusCode.OK, // 200
    Created = HttpStatusCode.Created, // 201

    BadRequest = HttpStatusCode.BadRequest, // 400
    Unauthorized = HttpStatusCode.Unauthorized, // 401
    NotFound = HttpStatusCode.NotFound, // 404
    Conflict = HttpStatusCode.Conflict, // 409

    Error = HttpStatusCode.InternalServerError, // 500
}
```

#### Utilizing HttpResult in APIs

`HttpResult` can be employed to indicate the outcome of API operations succinctly, mapping directly to HTTP response codes. Here's how it can be used to return different responses based on operation results:

```csharp
public HttpResult GetUser(int id) {
    var user = UserRepository.FindById(id);
    if (user == null) {
        return HttpResult.NotFound("User not found.");
    }
    return HttpResult.Ok(user);
}
```

#### Including Data in Responses

One of the key features of `HttpResult` is its ability to carry data along with the response status. This capability is particularly useful for returning entities or detailed error information in API responses:

```csharp
public HttpResult CreateUser(UserDto newUser) {
    var validationResult = ValidateNewUser(newUser);
    if (!validationResult.IsSuccess) {
        return HttpResult.BadRequest(validationResult.Errors);
    }
    var user = CreateUserFromDto(newUser);
    return HttpResult.Created(user);
}
```

#### Advantages of Using HttpResult

Employing `HttpResult` in web API development offers several benefits:
- **Consistency**: Ensures that API responses are standardized across different operations, facilitating easier consumption and integration.
- **Type Safety**: Reduces the risk of returning incorrect HTTP status codes, as responses are defined through explicit methods.
- **Simplicity**: Simplifies error handling and response construction, allowing developers to focus on the core logic of API operations.

- **SignInResult**: Designed for authentication processes.

### SignInResult for Authentication Outcomes

The `SignInResult` class is specifically tailored to encapsulate the outcomes of authentication processes, providing a clear and nuanced way to indicate the success or specific reasons for failure of sign-in attempts. This specialized result type ensures that authentication logic is both expressive and consistent.

```csharp
public enum SignInResultType {
    Error, // An internal error has occurred.
    Invalid, // request validation failed. No attempt was made.
    Failed, // attempt failed.
    Blocked, // account is blocked. (8 | 4) Counts as Failed.
    Locked, // account is locked. (16 | 4) Counts as Failed.
    Success, // attempt succeeded.
    ConfirmationPending, // attempt succeeded but email is not confirmed. (64 | 32).
    TwoFactorRequired, // attempt succeeded, but requires 2-factor authentication. (128 | 32).
}
```

#### Example Usage of SignInResult

```csharp
public SignInResult SignInUser(string username, string password) {
    var user = FindUserByUsername(username);
    if (user == null || !VerifyPassword(user, password)) {
        return SignInResult.Failed();
    }
    if (user.IsLockedOut) {
        return SignInResult.LockedOut();
    }
    // Additional sign-in logic here
    return SignInResult.Success();
}
```

#### Advantages of SignInResult

- **Clarity and Precision**: Distinguishes between various outcomes of sign-in attempts, such as failure due to invalid credentials or account lockout, enhancing error handling and user feedback.
- **Seamless Integration**: Fits neatly within the broader Result pattern framework, maintaining a consistent approach to handling operation outcomes across the application.
- **Customization and Extension**: Allows for the introduction of additional `SignInResultType` values to cater to complex authentication scenarios, offering flexibility and adaptability.

### System Utilities
System utilities offer essential abstractions over system resources such as Date and Time, GUIDs, File System, Assembly Information, and Console Input/Output. By enabling dependency injection, these abstractions support the creation of testable code, allowing the substitution of system classes with mock objects during testing.

- **DateTimeProvider**: Facilitates working with dates and times in a testable way by abstracting system-specific implementations.

#### Examples:
1. **Injecting DateTimeProvider into a service for testability:**
    ```csharp
    public class TimeSensitiveService {
        private readonly DateTimeProvider _dateTimeProvider;

        public TimeSensitiveService(DateTimeProvider dateTimeProvider) {
            _dateTimeProvider = dateTimeProvider;
        }

        public bool IsBusinessHour() {
            var currentHour = _dateTimeProvider.Now.Hour;
            return currentHour >= 9 && currentHour < 17;
        }
    }
    ```

2. **Mocking DateTimeProvider for unit tests:**
    ```csharp
    var fixedDateTime = Substitute.For<DateTimeProvider>();
    fixedDateTime.Now.Returns(new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero));
    
    var service = new TimeSensitiveService(fixedDateTime);
    var result = service.IsBusinessHour();
    
    result.Should().BeTrue(); // FluentAssertions used here
    ```

- **GuidProvider**: Allows generation of GUIDs that can be controlled in a testing environment.

#### Examples:
1. **Using GuidProvider to generate GUIDs in a service:**
    ```csharp
    public class IdentifierService {
        private readonly GuidProvider _guidProvider;

        public IdentifierService(GuidProvider guidProvider) {
            _guidProvider = guidProvider;
        }

        public Guid GetUniqueIdentifier() {
            return _guidProvider.New();
        }
    }
    ```

2. **Mocking GuidProvider in tests:**
    ```csharp
    var expectedGuid = Guid.NewGuid();
    var fixedGuidProvider = Substitute.For<GuidProvider>();
    fixedGuidProvider.New().Returns(expectedGuid);
    
    var service = new IdentifierService(fixedGuidProvider);
    var id = service.GetUniqueIdentifier();
    
    id.Should().Be(expectedGuid); // Asserting that the ID is indeed a GUID
    ```

- **FileSystem**: Provides an abstraction over file system operations, enabling better testing of file-related functionality.

#### Examples:
1. **Injecting FileSystem into a component:**
    ```csharp
    public class FileManager {
        private readonly FileSystem _fileSystem;

        public FileManager(FileSystem fileSystem) {
            _fileSystem = fileSystem;
        }

        public bool FileExists(string path) {
            return _fileSystem.FileExists(path);
        }
    }
    ```

2. **Mocking FileSystem during testing:**
    ```csharp
    var mockedFileSystem = Substitute.For<FileSystem>();
    mockedFileSystem.FileExists("test.txt").Returns(true);
    
    var fileManager = new FileManager(mockedFileSystem);
    var exists = fileManager.FileExists("test.txt");
    
    exists.Should().BeTrue(); // Validate the expected behavior
    ```

- **Input** and **Output**: Utilities for input and output operations, making console interactions testable.

#### Examples:
1. **Using Output in a service to write messages:**
    ```csharp
    public class ConsoleService {
        private readonly Input _input;
        private readonly Output _output;

        public ConsoleService(Input input, Output output) {
            _input = input;
            _output = output;
        }

        public string ReadUserInputOrEmpty() {
            return _input.ReadLine() ?? string.Empty;
        }

        public void DisplayMessage(string target) {
            _output.WriteLine($"Hello {target}!");
        }
    }
    ```

2. **Testing console input and output interactions:**
    ```csharp
    var mockedOutput = Substitute.For<Output>();
    var mockedInput = Substitute.For<Input>();
    var consoleService = new ConsoleService(mockedInput, mockedOutput);
    mockedInput.ReadLine().Returns("Test")

    var target = consoleService.ReadUserInputOrEmpty();
    var consoleService.DisplayMessage(target);

    outputMock.Received().WriteLine("Hello Test!"); // NSubstitute used to verify interaction
    ```

- **Assembly Information (AssemblyAccessor and AssemblyDescriptor)**: These utilities provide detailed access to assembly metadata, supporting a wide range of operations from version checking to attribute inspection.

Here is the information available through the `IAssemblyDescriptor` interface:
 - Name: The short name of the assembly.
 - FullName: The full name of the assembly, including the version, culture, and public key token.
 - Version: The version of the assembly.
 - RuntimeVersion: The runtime version of the assembly.
 - CultureInfo: The culture information of the assembly.
 - PublicKey: The public key of the assembly.
 - PublicKeyToken: The public key token of the assembly.
 - Location: The location of the assembly file.
 - EntryPoint: The entry point of the assembly.
 - ManifestModule: The manifest module of the assembly.
 - GetCustomAttributes() : Returns an array of all custom attributes applied to the assembly.
 - GetCustomAttributes(Type attributeType) : Returns an array of custom attributes applied to the assembly and identified by the specified type.
 - GetCustomAttributes<TAttribute>(): Returns an array of custom attributes applied to the assembly and identified by the specified type.
 - GetCustomAttribute<TAttribute>(): Returns the custom attribute applied to the assembly and identified by the specified type.
 - GetCustomAttribute(Type attributeType): Returns the custom attribute applied to the assembly and identified by the specified type.
 - DefinedTypes: Returns a collection of types defined in the assembly.
 - ExportedTypes: Returns a collection of types that are visible outside the assembly.
 - Modules: Returns a collection of modules in the assembly.

#### Examples:
1. **Retrieving Assembly Version:**
   ```csharp
   public class VersionService {
       private readonly IAssemblyAccessor _accessor;

       public VersionService(IAssemblyAccessor accessor) {
           _accessor = accessor;
       }

       public string GetCurrentAssemblyVersion() {
           return _accessor.GetExecutingAssembly().Version.ToString();
       }
   }
   ```

- **Environment Abstraction (Environment)**: Offers a unified interface to access various system utilities, facilitating easy mocking and dependency management.

#### Examples:
1. **Accessing System Utilities through Environment:**
   ```csharp
   public class ApplicationService {
       private readonly IEnvironment _environment;

       public ApplicationService(IEnvironment environment) {
           _environment = environment;
       }

       public void PerformOperation() {
           var assemblyVersion = _environment.Assembly.Version.ToString();
           var currentTime = _environment.DateTime.Now;
           // Use other system utilities as needed
       }
   }
   ```


### Pagination Utilities
Pagination utilities provide a standardized way to handle the slicing of large datasets into smaller, more manageable blocks or pages. These utilities can be used to define and access specific segments of data, often necessary when implementing APIs that support pagination.

- **IBlock / Block**: Interfaces and classes that represent a contiguous block of items with a specific size and an offset that indicates the starting point or key item.

- **IPage / Page**: Extends the concept of a block to include pagination-specific information, such as the total count of items, allowing for the calculation of the total number of pages.

- **BlockSettings**: Provides constants related to pagination, such as default block size and maximum allowable values.

#### Examples:
1. **Creating a Block of items:**
   ```csharp
   var items = new List<string> { "Item1", "Item2", "Item3" };
   var block = new Block<string>(items, "Item1");

   // block now contains a block of items starting from "Item1"
   ```

2. **Creating a Page with total count information:**
   ```csharp
   var items = new List<string> { "Item1", "Item2", "Item3" };
   var page = new Page<string>(items, 0, BlockSettings.DefaultBlockSize, 100);

   // page now contains a page of items with information about the total count of items
   ```

### Singleton and Options Patterns
Pattern utilities offer standardized ways to define and access special instances of classes, such as singleton, default, or empty instances. These utilities are designed to implement common design patterns in a consistent manner across your applications.

- **IHasInstance / HasInstance**: Allows for defining and accessing a singleton instance of a class.

- **IHasDefault / HasDefault**: Provides a way to access a default instance of a class.

- **IHasEmpty / HasEmpty**: Enables defining an 'empty' instance, often representing the absence of a value.

- **INamedOptions / NamedOptions**: Used for options classes with a standard way to define the section name used for configuration binding.

#### Examples:
1. **Accessing a singleton instance:**
    ```csharp
    public class SingletonService : HasInstance<SingletonService> {
        // Service implementation
    }

    var instance = SingletonService.Instance;
    ```

2. **Defining default options:**
    ```csharp
    public class ServiceOptions : NamedOptions<ServiceOptions> {
        public string ConnectionString { get; set; } = "DefaultConnectionString";
    }

    var defaultOptions = ServiceOptions.Default;
    ```

    Here is an example of the options class being used in a configuration binding scenario:

    ```json
    // In appsettings.json
    {
      // ...
      "Service": {
        "ConnectionString": "SomeConnectionString"
      }
      // ...
    }
    ```

    ```csharp
    public class Service {
        private readonly ServiceOptions _options;

        public Service(IOptions<ServiceOptions> options) {
            _options = options.Value;
        }

        public string GetConnectionString() {
            return _options.ConnectionString;
        }
    }

    // In Startup.cs
    services.Configure<ServiceOptions>(Configuration.GetSection("Service"));
    ```

3. **Using an empty instance:**
    ```csharp
    public class CommandResult : HasEmpty<CommandResult> {
        public bool Success { get; set; }
    }

    var noResult = CommandResult.Empty;
    ```

### Extension Classes
Extension classes provide additional methods to existing types, enhancing their capabilities and simplifying common operations.

- **Enumerable Extensions**: Extends `IEnumerable<T>` to provide additional transformation and collection generation methods.

#### Examples:
1. **Transforming IEnumerable to Array with custom projection:**
    ```csharp
    IEnumerable<int> numbers = new[] { 1, 2, 3, 4 };
    int[] squaredNumbers = numbers.ToArray(x => x * x);

    // squaredNumbers would be [1, 4, 9, 16]
    ```

2. **Creating a Dictionary from another IDictionary with a value transformation:**
    ```csharp
    IDictionary<string, int>> original = new Dictionary<string, int> {
        ["one"] = 1,
        ["two"] = 2
    };
    var newDictionary = original.ToDictionary(v => v * 10); // Multiplies each value by 10

    // newDictionary would have values [10, 20]
    ```

- **Queryable Extensions**: Provides projection and conversion extensions for `IQueryable<T>`.

#### Examples:
1. **Projecting IQueryable to Array:**
    ```csharp
    IQueryable<Person> people = GetPeopleQuery();
    PersonDto[] dtos = people.ToArray(person => new PersonDto(person.Name, person.Age));

    // dtos contains PersonDto objects projected from the Person source
    ```

2. **Transforming IQueryable into HashSet with projection:**
    ```csharp
    IQueryable<string> names = GetNamesQuery();
    HashSet<string> upperCaseNames = names.ToHashSet(name => name.ToUpper());

    // upperCaseNames contains upper case versions of the names from the source
    ```

- **Task Extensions**: Adds methods to `Task` and `ValueTask` to handle fire-and-forget scenarios with optional exception handling callbacks.

#### Examples:
1. **Using FireAndForget with a Task:**
    ```csharp
    Task someTask = DoWorkAsync();
    someTask.FireAndForget(); // No need to await, exceptions will be ignored
    ```

2. **Handling exceptions with FireAndForget:**
    ```csharp
    async Task DoWorkAsync() {
        // Simulate work
        await Task.Delay(1000);
        throw new InvalidOperationException("Something went wrong");
    }

    Task anotherTask = DoWorkAsync();
    anotherTask.FireAndForget(
        onException: ex => LogError(ex) // Log error if exception occurs
    );
    ```

### Other Utilities
Other utilities provide additional functionalities and helpers to extend existing concepts, making them more convenient and robust for everyday use.

- **Ensure**: Offers a variety of methods to ensure arguments meet certain conditions, throwing exceptions if they do not. This utility is very useful for validating method inputs and maintaining contracts within the code.
You can add the `Ensure` class as a static using to simplify the call. 

#### Examples:
1. **Using Ensure to validate arguments:**
    ```csharp
    public void ProcessData(string input) {
        input = Ensure.IsNotNullOrWhiteSpace(input);
        // Rest of the method logic
    }
    ```

2. **Using Ensure as a static using:**
    ```csharp
    using static DotNetToolbox.Ensure;
    
    // class dedinition and other code
    
    public void ProcessList(IList<string> items) {
        items = HasNoNullOrWhiteSpace(items);

        // Rest of the method logic
    }
    ```

- **CreateInstance**: Provides methods to dynamically create instances of types, which can be very helpful for creating types with non-public constructors or when types need to be created based on runtime data.

#### Examples:
1. **Creating an instance of a class with private constructor:**
    ```csharp
    var instance1 = CreateInstance.Of<SomeClassWithPrivateConstructor>();
    var instance2 = CreateInstance.Of<SomeClassWithArguments>(arg1, arg2);
    ```

2. **Creating an instance using dependency injection:**
    ```csharp
    public class SomeClassWithArgumentsAndServices {
        public SomeClassWithArgumentsAndServices(string arg1, string arg2, ILoggerFactory loggerFactory) {
            // ...
        }
    }
    
    // ...
    
    var serviceProvider = // ... build your service provider
    var instance = CreateInstance.Of<SomeClassWithArgumentsAndServices>(serviceProvider, arg1, arg2); // the injected services do not need to be added.
    ```

- **UrlSafeBase64String**: A record struct that represents a base64 encoded string safe for URL use. It can be used to encode and decode bytes, strings, and GUIDs to a URL-safe base64 format.

#### Examples:
1. **Encoding and decoding a URL-safe base64 string:**
    ```csharp
    byte[] original = { 0x1, 0x2, 0x3 };
    UrlSafeBase64String encoded = original;
    byte[] decoded = encoded;

    encoded.Should().BeOfType<string>(); // Outputs URL-safe base64 string
    decoded.Should().BeEquivalentTo(original); // Outputs True
    ```

2. **Working with GUIDs and base64 strings:**
    ```csharp
    Guid original = Guid.NewGuid();
    UrlSafeBase64String encoded = original;
    Guid decoded = encoded;

    encoded.Should().BeOfType<string>(); // Outputs URL-safe base64 string
    decoded.Should().Be(original); // Outputs True
    ```

- **IValidatable**: An interface that classes can implement to provide custom validation logic, returning a `Result` that indicates whether the validation was successful or not.

#### Examples:
1. **Implementing IValidatable in a class:**
   ```csharp
   using static DotNetToolbox.Result;

   public class Person : IValidatable {
       public string? Name { get; set; }
       public int Age { get; set; }

       public Result Validate(IDictionary<string, object?>? context = null) {
           var result = Success();
           if (string.IsNullOrWhiteSpace(Name)) {
               result += InvalidData(nameof(Name), "Name cannot be empty or whitespace.");
           }

           if (Age < 0 || Age > 120) {
               result += InvalidData(nameof(Age), "Age must be between 0 and 120.");
           }

           return result; // may return multiple errors
       }
   }
   ```

2. **Using Ensure to validate an IValidatable object:**
   ```csharp
   using static DotNetToolbox.Ensure;

   public void ProcessPerson(Person person) {
       person = IsValid(person); // throws ValidationException if validation fails.

       // Rest of the method logic
   }
   ```

- **DisposableStateHolder**: A utility designed to hold and manage the lifecycle of a disposable state. It ensures that if the state is disposable, it is properly disposed of, simplifying resource management and cleanup.

#### Examples:
1. **Using DisposableStateHolder with Disposable State:**
   ```csharp
   var disposableResource = new MemoryStream();
   using var stateHolder = new DisposableStateHolder(disposableResource);
   // The MemoryStream will be disposed of when stateHolder is disposed.
   ```


- **Indexed**: This struct pairs elements with their indices, enhancing the handling of sequences by keeping track of each element's position. It's invaluable for operations requiring index information alongside elements.

#### Examples:
1. **Enumerating Items with Indices:**
   ```csharp
   var items = new[] { "apple", "banana", "cherry" };
   var indexedItems = items.Select((value, index) => new Indexed<string>(index, value));
   foreach (var item in indexedItems) {
       Console.WriteLine($"Item {item.Index}: {item.Value}");
   }
   ```
