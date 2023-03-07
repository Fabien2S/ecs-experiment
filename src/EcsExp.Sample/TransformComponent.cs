using System.Numerics;

namespace EcsExp.Sample;

public struct TransformComponent
{
    public Matrix4x4 LocalToWorldMatrix;

    public TransformComponent()
    {
        LocalToWorldMatrix = Matrix4x4.Identity;
    }
}