global using System.Diagnostics.CodeAnalysis;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;

global using DotNetToolbox.ConsoleApplication.Application;
global using DotNetToolbox.ConsoleApplication.Arguments;
global using DotNetToolbox.ConsoleApplication.Commands;
global using DotNetToolbox.ConsoleApplication.Exceptions;
global using DotNetToolbox.ConsoleApplication.Nodes;
global using DotNetToolbox.ConsoleApplication.Utilities;
global using DotNetToolbox.Environment;
global using DotNetToolbox.Results;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Configuration.Json;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.FileProviders;
global using Microsoft.Extensions.Logging;

global using RadLine;
global using RadLine.Commands;

global using Spectre.Console;

global using static System.Ensure;
global using static System.Text.RegularExpressions.RegexOptions;
global using static DotNetToolbox.ConsoleApplication.Utilities.OutputFormatter;
global using static DotNetToolbox.Results.Result;
global using static DotNetToolbox.Results.ValidationResult;
