namespace EcsExp.ECS.Archetypes;

public interface IEcsArchetype
{
    static abstract int ComponentCount { get; }

    static abstract uint ComponentSize(int index);


    static abstract int OffsetOf<TComponent>() where TComponent : unmanaged;
}