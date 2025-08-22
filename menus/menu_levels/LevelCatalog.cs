using Godot;
using System;
using System.Collections.Generic;

public static class LevelCatalog
{
    private const string LevelsDir = "res://resources/levels";
    public static IEnumerable<LevelDataResource> GetAll() => _levels?.Values;
    private static Dictionary<string, LevelDataResource> _levels;

    private static string NormalizePath(string fileName)
    {
        string name = fileName.EndsWith(".remap", StringComparison.OrdinalIgnoreCase)
            ? fileName.Substring(0, fileName.Length - ".remap".Length)
            : fileName;
        return $"{LevelsDir}/{name}";
    }

    public static void LoadAll()
    {
        _levels = new Dictionary<string, LevelDataResource>();

        var dir = DirAccess.Open(LevelsDir);
        if (dir == null)
        {
            Log.Err($"Could not open {LevelsDir}");
            return;
        }

        dir.ListDirBegin();
        string fileName;
        while (!string.IsNullOrEmpty(fileName = dir.GetNext()))
        {
            if (dir.CurrentIsDir()) continue;
            if (fileName.StartsWith(".")) continue;

            bool ok =
                fileName.EndsWith(".tres", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".res", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".tres.remap", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".res.remap", StringComparison.OrdinalIgnoreCase);

            if (!ok) continue;

            string fullPath = NormalizePath(fileName);
            var res = ResourceLoader.Load(fullPath);
            if (res == null)
            {
                Log.Err($"ResourceLoader.Load returned null {fullPath}");
                continue;
            }

            if (res is not LevelDataResource level)
            {
                Log.Err($"Not a LevelDataResource {fullPath} (got {res.GetClass()})");
                continue;
            }

            if (string.IsNullOrEmpty(level.Key))
            {
                Log.Err($"Level Resource missing Key: {fullPath}");
                continue;
            }

            _levels[level.Key] = level;
        }
        dir.ListDirEnd();
    }

    public static LevelDataResource Get(string levelId)
    {
        if (_levels == null)
        {
            Log.Err("LoadAll() was not called before Get()");
            return null;
        }
        return _levels.TryGetValue(levelId, out var level) ? level : null;
    }
}
