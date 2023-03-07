using System.Runtime.CompilerServices;

namespace EcsExp.ECS.Entities;

public readonly struct EcsEntityState
{
    public readonly ushort Version;
    public readonly ushort StorageIndex;

    public EcsEntityState(ushort version, ushort storageIndex)
    {
        Version = version;
        StorageIndex = storageIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Match(in EcsEntity entity) => entity.Version == Version;
}