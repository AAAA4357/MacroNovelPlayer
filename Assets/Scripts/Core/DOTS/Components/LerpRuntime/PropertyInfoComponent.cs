using Unity.Collections;
using Unity.Entities;

public struct PropertyInfoComponent : IComponentData
{
    public NativeArray<float> LerpKeyArray;
    public NativeArray<bool> LerpEnabledArray;
    public float StartTime;
    public float EndTime;
}
