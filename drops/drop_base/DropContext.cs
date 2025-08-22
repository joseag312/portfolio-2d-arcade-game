using Godot;

public class DropContext
{
    public DropSourceType SourceType = DropSourceType.Common;
}

public enum DropSourceType
{
    Common,
    Rare,
    Epic,
    Legend,
    Boss
}