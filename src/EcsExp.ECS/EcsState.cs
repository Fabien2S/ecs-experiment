namespace EcsExp.ECS;

public readonly ref struct EcsState
{
    public readonly float DeltaTime;

    public EcsState(float deltaTime)
    {
        DeltaTime = deltaTime;
    }
}