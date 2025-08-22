#nullable enable
using System;

public interface IDropAmount
{
    void SetAmount(int amount, DropContext? context = null);
}
