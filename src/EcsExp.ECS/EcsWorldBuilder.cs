using EcsExp.ECS.Archetypes;
using EcsExp.ECS.Buffers;
using EcsExp.ECS.Systems;

namespace EcsExp.ECS;

public unsafe class EcsWorldBuilder
{
    private readonly List<IEcsBuffer> _buffers;
    private readonly delegate*<EcsWorld, in EcsState, EcsSystemResult>[] _systems;

    public EcsWorldBuilder()
    {
        _buffers = new List<IEcsBuffer>();
        _systems = new delegate*<EcsWorld, in EcsState, EcsSystemResult>[0];
    }

    private EcsWorldBuilder AddSystem(delegate*<EcsWorld, in EcsState, EcsSystemResult> system)
    {
        _systems[^1] = system;
        return this;
    }

    public EcsWorldBuilder AddSystem<TSystem>() where TSystem : IEcsSystem
    {
        return AddSystem(&TSystem.Process);
    }

    public EcsWorldBuilder AddArchetype<TArchetype>() where TArchetype : IEcsArchetype
    {
        _buffers.Add(new EcsBuffer<TArchetype>());
        return this;
    }

    public EcsWorld Build()
    {
        return new EcsWorld(
            _buffers.ToArray(),
            _systems
        );
    }
}