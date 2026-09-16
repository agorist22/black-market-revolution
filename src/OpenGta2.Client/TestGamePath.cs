using System;
using System.IO;

namespace OpenGta2.Client;

/// <summary>
/// Resolves the legal GTA2 install root from OPENGTA2_PATH (User or Process env).
/// That folder must contain game data such as data/bil.gmp and data/bil.sty.
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
                $"{EnvironmentVariableName} is not set. Set a User environment variable to your legal GTA2 install root " +
                "(the folder that contains data\\bil.gmp). See docs/BUILD-WINDOWS.md. " +
                "Do not use the Start Menu shortcuts folder.";
            return false;
        }

        if (!System.IO.Directory.Exists(path))
        {
            error = $"{EnvironmentVariableName} is set to '{path}', but that directory does not exist.";
            return false;
        }

        var marker = Path.Combine(path, "data", "bil.gmp");
        if (!File.Exists(marker))
        {
            error =
                $"{EnvironmentVariableName} is set to '{path}', but data\\bil.gmp was not found. " +
                "Point it at the GTA2 install root (parent of the data folder), not Start Menu or a shortcut.";
            return false;
        }

        root = path;
        return true;
    }

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
