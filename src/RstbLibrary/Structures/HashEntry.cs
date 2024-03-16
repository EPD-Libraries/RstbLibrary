namespace RstbLibrary.Structures;

public readonly struct HashEntry
{
    public const int SIZE = 8;

    public readonly uint Hash;
    public readonly uint Value;

    public readonly void Deconstruct(out uint hash, out uint value)
    {
        hash = Hash;
        value = Value;
    }
}
