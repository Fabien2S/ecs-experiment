namespace EcsExp.ECS.Entities;

public readonly struct EcsEntity : IComparable<EcsEntity>, IEquatable<EcsEntity>
{
    internal const int MaxEntities = 65536;

    internal readonly ushort Index;
    internal readonly ushort Version;

    public EcsEntity(ushort index, ushort version)
    {
        Index = index;
        Version = version;
    }

    public int CompareTo(EcsEntity other)
    {
        var compareTo = Index.CompareTo(other.Index);
        return compareTo == 0 ? Version.CompareTo(other.Version) : compareTo;
    }

    public bool Equals(EcsEntity other)
    {
        return Index == other.Index && Version == other.Version;
    }

    public override bool Equals(object? obj)
    {
        return obj is EcsEntity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Index << sizeof(ushort) | Version;
    }

    public override string ToString()
    {
        return $"Entity {Index} (rev {Version})";
    }

    public static bool operator ==(EcsEntity x, EcsEntity y) => x.Equals(y);

    public static bool operator !=(EcsEntity x, EcsEntity y) => !x.Equals(y);
}