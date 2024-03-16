using Revrs;
using RstbLibrary.Structures;
using RstbLibrary.Tables;
using System.Buffers.Binary;

namespace RstbLibrary;

public readonly ref struct ImmutableRstb
{
    internal const int FIXED_OVERFLOW_KEY_SIZE = 0x80;
    internal const int FIXED_OVERFLOW_ENTRY_SIZE = FIXED_OVERFLOW_KEY_SIZE + 4;

    public readonly RstbVersion Version;
    public readonly Endianness Endianness;

    public readonly ImmutableHashTable HashTable;
    public readonly ImmutableOverflowTable OverflowTable;

    public ImmutableRstb(ref RevrsReader reader)
    {
        if (reader.Data[0..4].SequenceEqual("RSTB"u8)) {
            ref RstbHeader header = ref reader.Read<RstbHeader, RstbHeader.Reverser>();
            Read(ref reader, ref header, out HashTable, out OverflowTable);
            Version = RstbVersion.Fixed;
        }
        else if (reader.Data[0..6].SequenceEqual("RESTBL"u8)) {
            ref RestblHeader header = ref reader.Read<RestblHeader, RestblHeader.Reverser>();
            Read(ref reader, ref header, out HashTable, out OverflowTable);
            Version = header.Version;
        }
        else {
            throw new InvalidDataException($"""
                Invalid RSTB magic, the magic did not match 'RESTBL' or 'RSTB'
                """);
        }

        Endianness = reader.Endianness;
    }

    private static void Read(ref RevrsReader reader, ref RstbHeader header, out ImmutableHashTable hashTable, out ImmutableOverflowTable overflowTable)
    {
        int hashTableSize = header.HashTableCount * HashEntry.SIZE;
        int overflowTableSize = header.OverflowTableCount * FIXED_OVERFLOW_ENTRY_SIZE;
        int expectedFileSize = RstbHeader.SIZE + hashTableSize + overflowTableSize;

        if (reader.Length != expectedFileSize) {
            reader.Endianness = (Endianness)BinaryPrimitives
                .ReverseEndianness((ushort)reader.Endianness);
            reader.Reverse<RstbHeader, RstbHeader.Reverser>(0);

            hashTableSize = header.HashTableCount * HashEntry.SIZE;
            overflowTableSize = header.OverflowTableCount * FIXED_OVERFLOW_ENTRY_SIZE;
            expectedFileSize = RstbHeader.SIZE + hashTableSize + overflowTableSize;

            if (reader.Length != expectedFileSize) {
                throw new InvalidDataException($"""
                    Invalid RSTB file, expected a length of '{expectedFileSize}' but found '{reader.Length}'
                    """);
            }
        }

        if (reader.Endianness.IsNotSystemEndianness()) {
            ImmutableHashTable.Reverse(ref reader, header.HashTableCount);
            ImmutableOverflowTable.Reverse(ref reader, header.OverflowTableCount, FIXED_OVERFLOW_KEY_SIZE);
        }

        hashTable = new(reader.Data[RstbHeader.SIZE..(hashTableSize += RstbHeader.SIZE)]);
        overflowTable = new(reader.Data[hashTableSize..(hashTableSize + overflowTableSize)], FIXED_OVERFLOW_KEY_SIZE);
    }

    private static void Read(ref RevrsReader reader, ref RestblHeader header, out ImmutableHashTable hashTable, out ImmutableOverflowTable overflowTable)
    {
        int hashTableSize = header.HashTableCount * HashEntry.SIZE;
        int overflowTableSize = header.OverflowTableCount * (header.OverflowKeySize + sizeof(uint));
        int expectedFileSize = RestblHeader.SIZE + hashTableSize + overflowTableSize;

        if (reader.Length != expectedFileSize) {
            reader.Endianness = (Endianness)BinaryPrimitives
                .ReverseEndianness((ushort)reader.Endianness);
            reader.Reverse<RestblHeader, RestblHeader.Reverser>(0);

            hashTableSize = header.HashTableCount * HashEntry.SIZE;
            overflowTableSize = header.OverflowTableCount * (header.OverflowKeySize + sizeof(uint));
            expectedFileSize = RestblHeader.SIZE + hashTableSize + overflowTableSize;

            if (reader.Length != expectedFileSize) {
                throw new InvalidDataException($"""
                    Invalid RSTB file, expected a length of '{expectedFileSize}' but found '{reader.Length}'
                    """);
            }
        }

        if (reader.Endianness.IsNotSystemEndianness()) {
            ImmutableHashTable.Reverse(ref reader, header.HashTableCount);
            ImmutableOverflowTable.Reverse(ref reader, header.OverflowTableCount, header.OverflowKeySize);
        }

        hashTable = new(reader.Data[RestblHeader.SIZE..(hashTableSize += RestblHeader.SIZE)]);
        overflowTable = new(reader.Data[hashTableSize..(hashTableSize + overflowTableSize)], header.OverflowKeySize);
    }
}
