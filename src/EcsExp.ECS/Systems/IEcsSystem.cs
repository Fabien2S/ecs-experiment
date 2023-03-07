namespace EcsExp.ECS.Systems;

public interface IEcsSystem
{
    static abstract void Initialize(EcsWorld world);
    static abstract void Reset(EcsWorld world);

    static abstract EcsSystemResult Process(EcsWorld world, in EcsState state);
}