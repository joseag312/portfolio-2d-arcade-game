using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class PortraitLibrary : Resource
{
    [Export] public Godot.Collections.Array<PortraitEntry> Entries { get; set; } = new();

    private Dictionary<string, PortraitEntry> _portraitMap;

    private void BuildPortraitMap()
    {
        _portraitMap = new Dictionary<string, PortraitEntry>();

        foreach (var entry in Entries)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Key))
                _portraitMap[entry.Key] = entry;
        }
    }

    public PortraitEntry GetPortrait(string key)
    {
        if (_portraitMap == null)
            BuildPortraitMap();

        if (_portraitMap.TryGetValue(key, out var entry))
            return entry;

        Log.Err($"No Portrait found for '{key}'");
        return null;
    }
}
