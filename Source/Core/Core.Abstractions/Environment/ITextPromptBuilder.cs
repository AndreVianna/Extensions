namespace DotNetToolbox.Environment;

public interface ITextPromptBuilder {
    string Show();
    Task<string> ShowAsync(CancellationToken ct = default);
}
