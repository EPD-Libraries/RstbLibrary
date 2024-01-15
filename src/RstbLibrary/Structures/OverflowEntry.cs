namespace RstbLibrary.Structures;

public readonly ref struct OverflowEntry(Span<byte> key, uint value)
{
    public readonly Span<byte> Key = key;
    public readonly uint Value = value;

    public readonly void Deconstruct(out Span<byte> key, out uint value)
    {
        key = Key;
        value = Value;
    }
}
