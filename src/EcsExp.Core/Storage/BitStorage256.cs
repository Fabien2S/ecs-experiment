using System.Numerics;

namespace EcsExp.Storage;

public struct BitStorage256 : IEquatable<BitStorage256>
{
    private const int BitCount = byte.MaxValue + 1;
    private const byte SectionSize = sizeof(ulong) * 8;

    private const byte FirstSectionEnd = 1 * SectionSize;
    private const byte SecondSectionEnd = 2 * SectionSize;
    private const byte ThirdSectionEnd = 3 * SectionSize;

    public int Count => BitOperations.PopCount(_first) +
                        BitOperations.PopCount(_second) +
                        BitOperations.PopCount(_third) +
                        BitOperations.PopCount(_fourth);

    public bool IsFull => _first == ulong.MaxValue &&
                          _second == ulong.MaxValue &&
                          _third == ulong.MaxValue &&
                          _fourth == ulong.MaxValue;

    private ulong _first;
    private ulong _second;
    private ulong _third;
    private ulong _fourth;

    public void Clear(bool value)
    {
        _first = value ? ulong.MaxValue : ulong.MinValue;
        _second = value ? ulong.MaxValue : ulong.MinValue;
        _third = value ? ulong.MaxValue : ulong.MinValue;
        _fourth = value ? ulong.MaxValue : ulong.MinValue;
    }

    public bool Equals(BitStorage256 other)
    {
        return _first == other._first &&
               _second == other._second &&
               _third == other._third &&
               _fourth == other._fourth;
    }

    public override bool Equals(object? obj)
    {
        return obj is BitStorage256 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_first, _second, _third, _fourth);
    }

    public override string ToString()
    {
        return string.Create(BitCount, this, (span, storage) =>
        {
            for (var i = 0; i < BitCount; i++)
                span[i] = storage[(byte)i] ? '1' : '0';
        });
    }

    public bool this[byte index]
    {
        get
        {
            var mask = 1UL << (index % SectionSize);
            return index switch
            {
                < FirstSectionEnd => _first & mask,
                < SecondSectionEnd => _second & mask,
                < ThirdSectionEnd => _third & mask,
                _ => _fourth & mask
            } != 0;
        }
        set
        {
            var mask = 1UL << (index % SectionSize);
            switch (index)
            {
                case < FirstSectionEnd:
                    if (value) _first |= mask;
                    else _first &= ~mask;
                    break;
                case < SecondSectionEnd:
                    if (value) _second |= mask;
                    else _second &= ~mask;
                    break;
                case < ThirdSectionEnd:
                    if (value) _third |= mask;
                    else _third &= ~mask;
                    break;
                default:
                    if (value) _fourth |= mask;
                    else _fourth &= ~mask;
                    break;
            }
        }
    }

    public static BitStorage256 operator &(BitStorage256 a, BitStorage256 b)
    {
        return new BitStorage256
        {
            _first = a._first & b._first,
            _second = a._second & b._second,
            _third = a._third & b._third,
            _fourth = a._fourth & b._fourth,
        };
    }

    public static BitStorage256 operator |(BitStorage256 a, BitStorage256 b)
    {
        return new BitStorage256
        {
            _first = a._first | b._first,
            _second = a._second | b._second,
            _third = a._third | b._third,
            _fourth = a._fourth | b._fourth,
        };
    }

    public static bool operator ==(BitStorage256 left, BitStorage256 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BitStorage256 left, BitStorage256 right)
    {
        return !(left == right);
    }
}