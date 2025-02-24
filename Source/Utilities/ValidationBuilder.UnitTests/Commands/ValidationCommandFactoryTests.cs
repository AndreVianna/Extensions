namespace DotNetToolbox.ValidationBuilder.Commands;

public sealed class ValidationCommandFactoryTests {
    private static object?[] Args(params object?[] args) => args;

    [Theory]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(IValidatable))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(List<string>))]
    [InlineData(typeof(Dictionary<string, int>))]
    [InlineData(typeof(Dictionary<string, decimal>))]
    [InlineData(typeof(Dictionary<string, string>))]
    [InlineData(typeof(Dictionary<double, double>))]
    public void Create_ForUnsupportedValidator_Throws(Type subjectType) {
        //Act
        var action = () => ValidationCommandFactory.For(subjectType, "Attribute").Create("Anything");

        //Assert
        action.Should().Throw<InvalidOperationException>();
    }

    private const string _string = "AbcDef";
    private const int _integer = 42;
    private const decimal _decimal = 42.0m;
    private static readonly DateTime _dateTime = DateTime.Parse("2020-01-01 10:10:10.12345");
    private static readonly Type _type = typeof(string);
    private static readonly List<int> _integers = [1, 2, 3];
    private static readonly List<decimal> _decimals = [1.0m, 2.0m, 3.0m];
    private static readonly List<int?> _nullableIntegers = [1, 2, 3];
    private static readonly List<decimal?> _nullableDecimals = [1.0m, 2.0m, 3.0m];
    private static readonly List<string> _strings = ["A", _string, "C"];
    private static readonly Dictionary<string, int> _strings2Integers = new() { ["A"] = 1, ["B"] = 2, ["C"] = 3 };
    private static readonly Dictionary<string, decimal> _strings2Decimals = new() { ["A"] = 1m, ["B"] = 2m, ["C"] = 3m };
    private static readonly Dictionary<string, int?> _strings2NullableIntegers = new() { ["A"] = 1, ["B"] = 2, ["C"] = 3 };
    private static readonly Dictionary<string, decimal?> _strings2NullableDecimals = new() { ["A"] = 1m, ["B"] = 2m, ["C"] = 3m };
    private static readonly Dictionary<string, string> _strings2Strings = new() { ["A"] = "1", ["B"] = "2", ["C"] = "3" };

    private sealed class TestDataForValidateSuccess : TheoryData<string, object?[], object?, Type> {
        public TestDataForValidateSuccess() {
            Add("Contains", Args(_integers[1]), _integers, _integers.GetType());
            Add("Contains", Args(_string[1..^1]), _string, _string.GetType());
            Add("ContainsKey", Args(_strings2Strings.Keys.First()), _strings2Strings, _strings2Strings.GetType());
            Add("ContainsValue", Args(_strings2Integers.Values.Last()), _strings2Integers, _strings2Integers.GetType());
            Add("Has", Args(_integers.Count), _integers, _integers.GetType());
            Add("Has", Args(_decimals.Count), _decimals, _decimals.GetType());
            Add("Has", Args(_nullableIntegers.Count), _integers, _nullableIntegers.GetType());
            Add("Has", Args(_nullableDecimals.Count), _decimals, _nullableDecimals.GetType());
            Add("Has", Args(_strings.Count), _strings, _strings.GetType());
            Add("Has", Args(_strings2Integers.Count), _strings2Integers, _strings2Integers.GetType());
            Add("Has", Args(_strings2Decimals.Count), _strings2Decimals, _strings2Decimals.GetType());
            Add("Has", Args(_strings2NullableIntegers.Count), _strings2Integers, _strings2NullableIntegers.GetType());
            Add("Has", Args(_strings2NullableDecimals.Count), _strings2Decimals, _strings2NullableDecimals.GetType());
            Add("Has", Args(_strings2Strings.Count), _strings2Strings, _strings2Strings.GetType());
            Add("HasAtLeast", Args(_strings.Count), _strings, _strings.GetType());
            Add("HasAtLeast", Args(_strings2Strings.Count), _strings2Strings, _strings2Strings.GetType());
            Add("HasAtMost", Args(_strings.Count), _strings, _strings.GetType());
            Add("HasAtMost", Args(_strings2Strings.Count), _strings2Strings, _strings2Strings.GetType());
            Add("IsAfter", Args(_dateTime), _dateTime.AddSeconds(1), _dateTime.GetType());
            Add("IsBefore", Args(_dateTime), _dateTime.AddSeconds(-1), _dateTime.GetType());
            Add("IsEmpty", Args(), _integers.Where(_ => false).ToList(), _integers.GetType());
            Add("IsEmpty", Args(), _strings2Integers.Where(_ => false).ToDictionary(k => k.Key, v => v.Value), _strings2Integers.GetType());
            Add("IsEqualTo", Args(_dateTime), _dateTime, _dateTime.GetType());
            Add("IsEqualTo", Args(_decimal), _decimal, _decimal.GetType());
            Add("IsEqualTo", Args(_integer), _integer, _integer.GetType());
            Add("IsEqualTo", Args(_integers), new List<int> { 1, 3, 2 }, _integers.GetType());
            Add("IsEqualTo", Args(_string), _string, _string.GetType());
            Add("IsEqualTo", Args(_strings), _strings, _strings.GetType());
            Add("IsEqualTo", Args(_type), _type, typeof(Type));
            Add("IsGreaterThan", Args(_decimal), _decimal + 0.01m, _decimal.GetType());
            Add("IsGreaterThan", Args(_integer), _integer + 1, _integer.GetType());
            Add("IsIn", Args(_strings.OfType<object?>().ToArray()), _string, _string.GetType());
            Add("IsLessThan", Args(_decimal), _decimal - 0.01m, _decimal.GetType());
            Add("IsLessThan", Args(_integer), _integer - 1, _integer.GetType());
            Add("IsNull", Args(), default(string), typeof(string));
            Add("LengthIs", Args(_string.Length), _string, _string.GetType());
            Add("LengthIsAtLeast", Args(_string.Length), _string, _string.GetType());
            Add("LengthIsAtMost", Args(_string.Length), _string, _string.GetType());
        }
    }

    [Theory]
    [ClassData(typeof(TestDataForValidateSuccess))]
    public void Validate_WithValidSubject_ReturnsSuccess(string validatorName, object?[] args, object? validValue, Type valueType) {
        var validator = ValidationCommandFactory.For(valueType, "Attribute").Create(validatorName, args);
        var validResult = validator.Validate(validValue);

        validResult.IsSuccess.Should().BeTrue();
    }

    private sealed class TestDataForValidateFailure : TheoryData<string, object?[], object?, Type> {
        public TestDataForValidateFailure() {
            Add("IsNull", Args(), "NotNull", typeof(string));
            Add("IsEmpty", Args(), _integers, _integers.GetType());
            Add("IsEmpty", Args(), _strings2Integers, _strings2Integers.GetType());
            Add("Contains", Args("Xyz"), _string, _string.GetType());
            Add("Contains", Args(13), _integers, _integers.GetType());
            Add("ContainsKey", Args("Nope"), _strings2Strings, _strings2Strings.GetType());
            Add("ContainsValue", Args(13), _strings2Integers, _strings2Integers.GetType());
            Add("Has", Args(99), _integers, _integers.GetType());
            Add("Has", Args(99), _strings, _strings.GetType());
            Add("Has", Args(99), _strings2Decimals, _strings2Decimals.GetType());
            Add("Has", Args(99), _strings2Integers, _strings2Integers.GetType());
            Add("Has", Args(99), _strings2Strings, _strings2Strings.GetType());
            Add("IsAfter", Args(_dateTime), _dateTime.AddSeconds(-1), _dateTime.GetType());
            Add("IsBefore", Args(_dateTime), _dateTime.AddSeconds(1), _dateTime.GetType());
            Add("IsEqualTo", Args(_integer), _integers, _integers.GetType());
            Add("IsEqualTo", Args(_integers), _integer, _integer.GetType());
            Add("IsEqualTo", Args(_integers), new List<int> { 1, 2 }, _integers.GetType());
            Add("IsEqualTo", Args(_integers), new List<int> { 1, 2, 4 }, _integers.GetType());
            Add("IsEqualTo", Args(_integers), new List<int> { 1, 2, 3, 3 }, _integers.GetType());
            Add("IsEqualTo", Args(new List<int> { 1, 3, 2, 2 }), new List<int> { 1, 2, 3, 3 }, _integers.GetType());
            Add("IsEqualTo", Args(_integers), new List<string> { "1", "2", "2" }, _integers.GetType());
            Add("IsEqualTo", Args(_string), "Nope", _string.GetType());
            Add("IsEqualTo", Args(_integer), 13, _integer.GetType());
            Add("IsEqualTo", Args(_decimal), 13.01m, _decimal.GetType());
            Add("IsEqualTo", Args(_dateTime), _dateTime.AddDays(-1), _dateTime.GetType());
            Add("IsEqualTo", Args(_type), typeof(long), typeof(Type));
            Add("IsGreaterThan", Args(_integer), _integer - 1, _integer.GetType());
            Add("IsGreaterThan", Args(_decimal), _decimal - 0.01m, _decimal.GetType());
            Add("IsLessThan", Args(_decimal), _decimal + 0.01m, _decimal.GetType());
            Add("IsLessThan", Args(_integer), _integer + 1, _integer.GetType());
            Add("IsIn", Args(_strings.OfType<object?>().ToArray()), "Nope", _string.GetType());
            Add("LengthIs", Args(2), _string, _string.GetType());
            Add("HasAtMost", Args(2), _strings2Strings, _strings2Strings.GetType());
            Add("HasAtMost", Args(2), _strings, _strings.GetType());
            Add("LengthIsAtMost", Args(2), _string, _string.GetType());
            Add("HasAtLeast", Args(99), _strings2Strings, _strings2Strings.GetType());
            Add("HasAtLeast", Args(99), _strings, _strings.GetType());
            Add("LengthIsAtLeast", Args(99), _string, _string.GetType());
        }
    }

    [Theory]
    [ClassData(typeof(TestDataForValidateFailure))]
    public void Validate_WithInvalidSubject_ReturnsFailure(
        string validatorName,
        object?[] args,
        object invalidValue,
        Type valueType) {
        var validator = ValidationCommandFactory.For(valueType, "Attribute").Create(validatorName, args);
        var invalidResult = validator.Validate(invalidValue);

        invalidResult.IsInvalid.Should().BeTrue();
    }

    private sealed class TestCommand() : ValidationCommand("Source");

    [Fact]
    public void Validate_WithDefaultCommand_ReturnsSuccess() {
        var command = new TestCommand();

        var result = command.Validate("Value");

        result.IsSuccess.Should().BeTrue();
    }
}
