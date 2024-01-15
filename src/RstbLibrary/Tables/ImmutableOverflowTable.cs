using Revrs;
using Revrs.Extensions;
using RstbLibrary.Extensions;
using RstbLibrary.Structures;
using System.Runtime.CompilerServices;

namespace RstbLibrary.Tables;

public readonly ref struct ImmutableOverflowTable(Span<byte> data, int keySize)
{
    private readonly Span<byte> _data = data;
    private readonly int _keySize = keySize;
    private readonly int _entrySize = keySize + sizeof(uint);

    public readonly int Count {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data.Length / _entrySize;
    }

    public OverflowEntry this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            int offset = index * _entrySize;
            Span<byte> buffer = _data[offset..(offset + _entrySize)];
            return new(
                buffer[.._keySize], buffer[_keySize..].Read<uint>()
            );
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse(ref RevrsReader reader, int count, int keySize)
    {
        for (int i = 0; i < count; i++) {
            reader.Move(keySize);
            reader.Reverse<uint>();
        }
    }

    public SortedDictionary<string, uint> ToMutable()
    {
        SortedDictionary<string, uint> mutable = [];
        foreach ((var data, var value) in this) {
            mutable[data.ToManaged()] = value;
        }

        return mutable;
    }

    public readonly Enumerator GetEnumerator()
        => new(this);

    public ref struct Enumerator(ImmutableOverflowTable immutableOverflowTable)
    {
        private readonly ImmutableOverflowTable _immutableOverflowTable = immutableOverflowTable;
        private int _index = -1;

        public readonly OverflowEntry Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _immutableOverflowTable[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++_index < _immutableOverflowTable.Count;
        }
    }
}
