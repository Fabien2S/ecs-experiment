using EcsExp.ECS.Archetypes;
using EcsExp.Sample;

public unsafe class CubeArchetype : IEcsArchetype
{
    public static int ComponentCount => 1;

    public static int OffsetOf<TComponent>() where TComponent : unmanaged
    {
        var type = typeof(TComponent);
        if (type == typeof(TransformComponent))
            return 0;

        return -1;
    }

    public static uint ComponentSize(int index)
    {
        if (index == 0)
            return (uint)sizeof(TransformComponent);

        return 0;
    }
}