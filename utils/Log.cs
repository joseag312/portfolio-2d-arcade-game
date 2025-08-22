using Godot;
using System;
using System.Runtime.CompilerServices;

public static class Log
{
    private static string ShortFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return "Unknown";
        return System.IO.Path.GetFileNameWithoutExtension(path) ?? "Unknown";
    }

    public static void Err(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        GD.PrintErr($"{ShortFile(filePath)}:{lineNumber} - {message}");
    }

    public static void Dbg(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        GD.Print($"DEBUG: {ShortFile(filePath)}:{lineNumber} - {message}");
    }
}
