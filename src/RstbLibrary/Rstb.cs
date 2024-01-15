using Revrs;
using RstbLibrary.Helpers;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace RstbLibrary;

public enum RstbVersion : int
{
    /// <summary>
    /// RSTB with a fixed (0x80) overflow table (RSTB)
    /// </summary>
    Fixed,

    /// <summary>
    /// RSTB with a dynamic overflow table (RESTBL)
    /// </summary>
    Dynamic,
}

public sealed class Rstb
{
    public RstbVersion Version { get; set; } = RstbVersion.Dynamic;
    public Endianness Endianness { get; set; } = Endianness.Little;
    public SortedDictionary<uint, uint> HashTable { get; set; } = [];
    public SortedDictionary<string, uint> OverflowTable { get; set; } = [];

    public static Rstb FromBinary(Span<byte> data)
    {
        RevrsReader reader = RevrsReader.Native(data);
        ImmutableRstb rstb = new(ref reader);
        return FromImmutable(rstb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rstb FromImmutable(in ImmutableRstb rstb) => new(rstb);

    public Rstb()
    {
        
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Rstb(in ImmutableRstb rstb)
    {
        Version = rstb.Version;
        Endianness = rstb.Endianness;
        HashTable = rstb.HashTable.ToMutable();
        OverflowTable = rstb.OverflowTable.ToMutable();
    }

    public byte[] ToBinary(Endianness? endianness = null, RstbVersion? version = null, bool optimize = false)
    {
        using MemoryStream ms = new();
        WriteBinary(ms, endianness, version, optimize);
        return ms.ToArray();
    }

    public void WriteBinary(in Stream stream, Endianness? endianness = null, RstbVersion? version = null, bool optimize = false)
    {
        endianness ??= Endianness;
        version ??= Version;

        RevrsWriter writer = new(stream, endianness.Value);
        if (optimize) {
            OptimizeOverflowTable();    
        }

        switch (version) {
            case RstbVersion.Fixed: {
                WriteBinaryFixed(writer);
                break;
            }
            case RstbVersion.Dynamic: {
                WriteBinaryDynamic(writer);
                break;
            }
            default: {
                throw new NotSupportedException($"""
                    Unsupported RSTB version: '{Version}'
                    """);
            }
        }
    }

    private void WriteBinaryFixed(in RevrsWriter writer)
    {
        writer.Write("RSTB"u8);
        writer.Write(HashTable.Count);
        writer.Write(OverflowTable.Count);

        foreach ((var hash, var value) in HashTable.OrderBy(x => x.Key)) {
            writer.Write(hash);
            writer.Write(value);
        }

        foreach ((var key, var value) in OverflowTable.OrderBy(x => x.Key)) {
            int distance = ImmutableRstb.FIXED_OVERFLOW_KEY_SIZE - key.Length;
            if (distance < 0) {
                throw new InvalidOperationException($"""
                    The overflow key '{key}' could not fit into a fixed RSTB file.
                    """);
            }

            writer.WriteStringUtf8(key);
            writer.Move(distance);
            writer.Write(value);
        }
    }

    private void WriteBinaryDynamic(in RevrsWriter writer)
    {
        int overflowKeySize = OverflowTable.Keys.Max(x => x.Length + 1);
        overflowKeySize += overflowKeySize.AlignUp(2);

        writer.Write("RESTBL"u8);
        writer.Write(Version);
        writer.Write(overflowKeySize);
        writer.Write(HashTable.Count);
        writer.Write(OverflowTable.Count);

        foreach ((var hash, var value) in HashTable.OrderBy(x => x.Key)) {
            writer.Write(hash);
            writer.Write(value);
        }

        foreach ((var key, var value) in OverflowTable.OrderBy(x => x.Key)) {
            writer.WriteStringUtf8(key);
            writer.Move(overflowKeySize - key.Length);
            writer.Write(value);
        }
    }

    /// <summary>
    /// Move any keys that don't need to be in
    /// the overflow table to the hash table.
    /// </summary>
    private void OptimizeOverflowTable()
    {
        KeyValuePair<string, uint>[] copy
            = ArrayPool<KeyValuePair<string, uint>>.Shared.Rent(OverflowTable.Count);
        OverflowTable.CopyTo(copy, 0);

        foreach ((var key, var value) in copy.AsSpan()[..OverflowTable.Count]) {
            uint hash = Crc32.Compute(key);
            if (HashTable.TryAdd(hash, value)) {
                OverflowTable.Remove(key);
            }
        }

        ArrayPool<KeyValuePair<string, uint>>.Shared.Return(copy);
    }
}
