namespace OpenGta2.DebugConsole;

public static class TestGamePath
{
    public const string EnvironmentVariableName = "OPENGTA2_PATH";

    public static DirectoryInfo Directory
    {
        get
        {
            var path = Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.User)
                       ?? Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.Process)
                       ?? Environment.GetEnvironmentVariable(EnvironmentVariableName);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException(
                    $"{EnvironmentVariableName} is not set. Point it at your legal GTA2 install root. See docs/BUILD-WINDOWS.md.");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(
                    $"{EnvironmentVariableName} is set to '{path}', but that directory does not exist.");
            }

            return new DirectoryInfo(path);
        }
    }

    public static Stream OpenFile(string path)
    {
        var full = Path.Combine(Directory.FullName, path);
        if (!File.Exists(full))
        {
            throw new FileNotFoundException($"Missing '{path}' under {EnvironmentVariableName}.", full);
        }

        return File.OpenRead(full);
    }
}
