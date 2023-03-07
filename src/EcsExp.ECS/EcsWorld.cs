using EcsExp.Logging;
using EcsExp.Storage;
using EcsExp.ECS.Archetypes;
using EcsExp.ECS.Buffers;
using EcsExp.ECS.Entities;
using EcsExp.ECS.Systems;
using Microsoft.Extensions.Logging;

namespace EcsExp.ECS;

public unsafe class EcsWorld
{
    private static readonly ILogger<EcsWorld> Logger = LogManager.Create<EcsWorld>();

    private readonly IEcsBuffer[] _buffers;
    private readonly delegate*<EcsWorld, in EcsState, EcsSystemResult>[] _systems;

    private readonly EcsAllocator _allocator;
    private readonly EcsEntityState[] _states;

    public EcsWorld(IEcsBuffer[] buffers, delegate*<EcsWorld, in EcsState, EcsSystemResult>[] systems)
    {
        ArgumentNullException.ThrowIfNull(buffers);
        ArgumentNullException.ThrowIfNull(systems);
        if (buffers.Length >= byte.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(buffers), "Buffers is larger than 255");

        _systems = systems;
        _buffers = buffers;

        _allocator = new EcsAllocator();
        _states = new EcsEntityState[EcsEntity.MaxEntities];
    }

    public void Process(in EcsState state)
    {
        for (var i = 0; i < _systems.Length; i++)
        {
            var result = _systems[i](this, in state);
            if (result != EcsSystemResult.Success)
                Logger.LogError("ECS system #{Index} failed: {Result}", i, result);
        }
    }

    /// <summary>
    /// Checks if the entity exists in the world
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <returns>Whether the entity exists</returns>
    public bool Exists(in EcsEntity entity)
    {
        var state = _states[entity.Index];
        return state.Match(in entity);
    }

    public EcsBuffer<TArchetype> Buffer<TArchetype>(out byte index) where TArchetype : IEcsArchetype
    {
        for (index = 0; index < _buffers.Length; index++)
        {
            var buffer = _buffers[index];
            if (buffer is EcsBuffer<TArchetype> tBuffer)
                return tBuffer;
        }

        throw new ArgumentException($"{nameof(TArchetype)} is not part of the world", nameof(TArchetype));
    }

    public EcsFilter Filter<TComponent>() where TComponent : unmanaged
    {
        var mask = new BitStorage256();
        for (byte i = 0; i < _buffers.Length; i++)
        {
            var buffer = _buffers[i];
            if (buffer.HasComponent<TComponent>())
                mask[i] = true;
        }

        return new EcsFilter(mask);
    }

    public EcsEntity CreateEntity<TArchetype>() where TArchetype : IEcsArchetype
    {
        var buffer = Buffer<TArchetype>(out var bufferIndex);
        if (!buffer.Allocate(out var index, out var version))
        {
            return default;
        }

        // entity instantiated
        return new EcsEntity(version, bufferIndex, index);
    }

    public void DestroyEntity(EcsEntity entity)
    {
        var buffer = _buffers[entity.BufferIndex];
        buffer.Free(entity.StorageIndex);
    }
}