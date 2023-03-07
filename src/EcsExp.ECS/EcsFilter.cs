using EcsExp.Storage;

namespace EcsExp.ECS;

public struct EcsFilter
{
    internal readonly BitStorage256 ArchetypeMask;

    public EcsFilter(BitStorage256 archetypeMask)
    {
        ArchetypeMask = archetypeMask;
    }

    public static EcsFilter operator &(EcsFilter x, EcsFilter y)
    {
        return new EcsFilter(x.ArchetypeMask & y.ArchetypeMask);
    }
}