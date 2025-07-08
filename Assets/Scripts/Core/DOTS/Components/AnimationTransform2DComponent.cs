using MNP.Core.DataStruct.Animations;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct AnimationTransform2DArrayComponent : IComponentData
{
    public int AnimationCount;

    public NativeArray<float> AnimationFrameStartArray;

    public NativeArray<float> AnimationFrameDurationArray;

    public NativeArray<float2x3> AnimationPathP0Array;

    public NativeArray<float2x3> AnimationPathP1Array;

    public NativeArray<float2x3> AnimationPathP2Array;

    public NativeArray<float2x3> AnimationPathP3Array;

    public NativeArray<float4> AnimationPositionEaseArray;

    public NativeArray<int> PositionEaseSpacingArray;

    public NativeArray<float4> AnimationRotationEaseArray;

    public NativeArray<int> RotationEaseSpacingArray;

    public NativeArray<float4> AnimationScaleEaseArray;
    
    public NativeArray<int> ScaleEaseSpacingArray;
}
