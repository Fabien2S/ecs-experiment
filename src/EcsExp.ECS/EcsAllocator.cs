using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace EcsExp.ECS;

public class EcsAllocator
{
    private const int BitCount = ushort.MaxValue + 1;

    private const int SectionSize = sizeof(ulong) * 8;
    private const ulong SectionFull = ulong.MaxValue;

    /// <summary>
    /// Gets the count of allocated entities
    /// </summary>
    public ushort Count => _count;

    /// <summary>
    /// Gets if the allocator if full
    /// </summary>
    public bool IsFull => _searchIndex == _sections.Length;

    private readonly ulong[] _sections;

    private uint _searchIndex;
    private ushort _count;

    public EcsAllocator()
    {
        _sections = new ulong[BitCount / SectionSize];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref ulong SectionFromId(in ushort id, out ulong mask)
    {
        mask = 1UL << id;
        return ref _sections[id / SectionSize];
    }

    /// <summary>
    /// Find the first unset bit
    /// </summary>
    /// <returns>The bit index</returns>
    /// <exception cref="InvalidOperationException">No bit was found</exception>
    public bool FindFreeIndex(out ushort index)
    {
        if (IsFull)
        {
            index = 0;
            return false;
        }

        var section = _sections[_searchIndex];
        Debug.Assert(section != SectionFull, "section != SectionFull");

        var leadingZero = BitOperations.LeadingZeroCount(section);
        if (leadingZero == 1) _searchIndex++;

        var baseIndex = _searchIndex * SectionSize;
        var sectionIndex = (ushort)(SectionSize - leadingZero);

        index = (ushort)(baseIndex + sectionIndex);
        return true;
    }

    public bool this[ushort index]
    {
        get => (SectionFromId(index, out var mask) & mask) == mask;
        set
        {
            ref var section = ref SectionFromId(index, out var mask);

            var isBitSet = (section & mask) == mask;
            if (isBitSet == value) // no changes
                return;

            if (value)
            {
                _count++;
                section |= mask;
            }
            else
            {
                _count--;
                section &= ~mask;

                // update the highest id if needed
                var sectionIndex = (uint)index / SectionSize;
                if (sectionIndex < _searchIndex)
                    _searchIndex = sectionIndex;
            }
        }
    }
}