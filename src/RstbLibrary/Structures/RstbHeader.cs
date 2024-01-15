using Revrs;
using System.Runtime.InteropServices;

namespace RstbLibrary.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0xC)]
public struct RstbHeader
{
    public const int SIZE = 0xC;

    public uint Magic;
    public int HashTableCount;
    public int OverflowTableCount;

    public class Reverser : IStructReverser
    {
        public static void Reverse(in Span<byte> slice)
        {
            slice[0x4..0x8].Reverse();
            slice[0x8..0xC].Reverse();
        }
    }
}
