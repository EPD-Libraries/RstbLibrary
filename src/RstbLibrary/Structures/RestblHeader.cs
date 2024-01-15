using Revrs;
using System.Runtime.InteropServices;

namespace RstbLibrary.Structures;

[StructLayout(LayoutKind.Explicit, Pack = 0, Size = 0x16)]
public struct RestblHeader
{
    public const int SIZE = 0x16;

    [FieldOffset(0x06)]
    public RstbVersion Version;

    [FieldOffset(0x0A)]
    public int OverflowKeySize;

    [FieldOffset(0x0E)]
    public int HashTableCount;

    [FieldOffset(0x12)]
    public int OverflowTableCount;

    public class Reverser : IStructReverser
    {
        public static void Reverse(in Span<byte> slice)
        {
            slice[0x06..0x0A].Reverse();
            slice[0x0A..0x0E].Reverse();
            slice[0x0E..0x12].Reverse();
            slice[0x12..0x16].Reverse();
        }
    }
}
