namespace OpenGta2.Data.UnitTests;

/// <summary>
/// Resolves OPENGTA2_PATH for integration-style GameData tests that read real GTA2 files.
/// </summary>
public static class TestGamePath
{
    public const string EnvironmentVariableName = "OPENGTA2_PATH";

    public static bool TryGetRoot(out string root, out string? error)
    {
        root = "";
        error = null;

        var path = Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.User)
                   ?? Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.Process)
                   ?? Environment.GetEnvironmentVariable(EnvironmentVariableName);

        if (string.IsNullOrWhiteSpace(path))
        {
            error =
                $"{EnvironmentVariableName} is not set. Skip these tests until a legal GTA2 install path is configured. See docs/BUILD-WINDOWS.md.";
            return false;
        }

        if (!Directory.Exists(path))
        {
            error = $"{EnvironmentVariableName} is set to '{path}', but that directory does not exist.";
            return false;
        }

        root = path;
        return true;
    }

    public static bool IsConfigured => TryGetRoot(out _, out _);

    public static DirectoryInfo Directory
    {
        get
        {
            if (!TryGetRoot(out var root, out var error))
            {
                throw new InvalidOperationException(error);
            }

            return new DirectoryInfo(root);
        }
    }

    public static Stream OpenFile(string path)
    {
        var full = Path.Combine(Directory.FullName, path);
        if (!File.Exists(full))
        {
            throw new FileNotFoundException(
                $"Missing '{path}' under {EnvironmentVariableName} ({Directory.FullName}).", full);
        }

        return File.OpenRead(full);
    }
}
