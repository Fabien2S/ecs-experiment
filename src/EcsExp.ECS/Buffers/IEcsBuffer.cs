namespace EcsExp.ECS.Buffers;

public interface IEcsBuffer : IDisposable
{
    bool Allocate(out ushort index);
    void Free(ushort index);

    bool HasComponent<T>() where T : unmanaged;
}