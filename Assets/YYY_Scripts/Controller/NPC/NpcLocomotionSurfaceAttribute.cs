using System;

public enum NpcLocomotionSurfaceScope
{
    ExternalFacade = 0,
    RuntimeOnly = 1,
    DebugOnly = 2
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class NpcLocomotionSurfaceAttribute : Attribute
{
    public NpcLocomotionSurfaceAttribute(NpcLocomotionSurfaceScope scope)
    {
        Scope = scope;
    }

    public NpcLocomotionSurfaceScope Scope { get; }
    public string Guidance { get; set; } = string.Empty;
}
