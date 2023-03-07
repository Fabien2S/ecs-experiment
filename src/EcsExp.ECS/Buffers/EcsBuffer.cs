using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EcsExp.ECS.Archetypes;

namespace EcsExp.ECS.Buffers;

/// <summary>
/// Stores an archetype memory buffer
/// </summary>
/// <typeparam name="TArchetype">The archetype</typeparam>
public unsafe class EcsBuffer<TArchetype> : IEcsBuffer where TArchetype : IEcsArchetype
{
    private readonly EcsAllocator _allocator;

    private readonly byte*[] _componentBuffers;
    private ushort _componentCapacity;

    public EcsBuffer()
    {
        _allocator = new EcsAllocator();

        _componentBuffers = new byte*[TArchetype.ComponentCount];
        _componentCapacity = 0;
    }

    ~EcsBuffer()
    {
        Dispose();
    }

    private void Grow(ushort extends)
    {
        var oldCapacity = _componentCapacity;

        // figure out the capacity and realloc
        var capacity = extends < _componentCapacity ? 2 * _componentCapacity : _componentCapacity + extends;
        _componentCapacity = capacity > ushort.MaxValue ? ushort.MaxValue : (ushort)capacity;

        // resize all buffers
        for (var i = 0; i < _componentBuffers.Length; i++)
        {
            var componentSize = TArchetype.ComponentSize(i);

            // realloc buffer
            var componentBufferSize = _componentCapacity * componentSize;
            var componentBuffer = (byte*)NativeMemory.Realloc(_componentBuffers[i], componentBufferSize);

            // zero out the memory
            var usedBufferSize = oldCapacity * componentSize;
            var initBlockAddress = Unsafe.Add<byte>(componentBuffer, (int)usedBufferSize);
            var initBlockByteCount = componentBufferSize - usedBufferSize;
            Unsafe.InitBlock(initBlockAddress, 0, initBlockByteCount);

            _componentBuffers[i] = componentBuffer;
        }
    }

    public bool HasComponent<T>() where T : unmanaged
    {
        return TArchetype.OffsetOf<T>() >= 0;
    }

    public bool Allocate(out ushort index)
    {
        if (!_allocator.FindFreeIndex(out index))
        {
            // storage is full
            return false;
        }

        // reserve index
        _allocator[index] = true;

        if (index < _componentCapacity)
        {
            // memory is already allocated
            return true;
        }

        // reallocation is required
        Debug.Assert(index == _componentCapacity);
        Grow(1);
        return true;
    }

    public void Free(ushort index)
    {
        _allocator[index] = false;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_componentCapacity == 0)
            return;

        _componentCapacity = 0;
        foreach (var buffer in _componentBuffers)
        {
            NativeMemory.Free(buffer);
        }
    }
}