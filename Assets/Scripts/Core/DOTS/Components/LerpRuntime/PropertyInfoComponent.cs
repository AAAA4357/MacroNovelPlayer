using Unity.Collections;
using Unity.Entities;

public struct PropertyInfoComponent : IComponentData
{
    public float StartTime;
    public float EndTime;
    public bool LerpEnabled;
}
