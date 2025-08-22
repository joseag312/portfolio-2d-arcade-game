public static class WeaponSlotNames
{
    public const string Basic = "basic";
    public const string Big = "big";
    public const string Special1 = "special_1";
    public const string Special2 = "special_2";
    public const string Special3 = "special_3";
    public const string Special4 = "special_4";

    public static string GetSpecialSlot(int index)
    {
        return $"special_{index}";
    }
}
