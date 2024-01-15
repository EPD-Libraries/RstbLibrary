using Revrs;
using Revrs.Extensions;
using RstbLibrary.Structures;
using System.Runtime.CompilerServices;

namespace RstbLibrary.Tables;

public readonly ref struct ImmutableHashTable(Span<byte> data)
{
    private readonly Span<HashEntry> _entries = data.ReadSpan<HashEntry>();

    public int Count {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _entries.Length;
    }

    public HashEntry this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _entries[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse(ref RevrsReader reader, int count)
    {
        for (int i = 0; i < count; i++) {
            reader.Reverse<uint>();
            reader.Reverse<uint>();
        }
    }

    public SortedDictionary<uint, uint> ToMutable()
    {
        SortedDictionary<uint, uint> mutable = [];
        foreach ((var hash, var value) in _entries) {
            mutable[hash] = value;
        }

        return mutable;
    }

    public readonly Enumerator GetEnumerator()
        => new(this);

    public ref struct Enumerator(ImmutableHashTable immutableHashTable)
    {
        private readonly ImmutableHashTable _immutableHashTable = immutableHashTable;
        private int _index = -1;

        public readonly HashEntry Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _immutableHashTable[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++_index < _immutableHashTable.Count;
        }
    }
}
