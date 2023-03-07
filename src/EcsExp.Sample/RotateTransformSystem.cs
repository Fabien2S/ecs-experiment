using EcsExp.ECS;
using EcsExp.ECS.Systems;

namespace EcsExp.Sample;

public class RotateTransformSystem : IEcsSystem
{
    private static EcsFilter _filter;

    public static void Initialize(EcsWorld world)
    {
        _filter = world.Filter<TransformComponent>();
    }

    public static void Reset(EcsWorld world)
    {
    }

    public static EcsSystemResult Process(EcsWorld world, in EcsState state)
    {
        // TODO Initialize and Reset are never called
        // world.Iterate(_filter);

        return EcsSystemResult.Failed;
    }
}