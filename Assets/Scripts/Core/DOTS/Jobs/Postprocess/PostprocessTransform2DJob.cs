using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(BakeReadyComponent), typeof(Object2DComponent))]
    public partial struct PostprocessTransform2DJob : IJobEntity
    {
        [ReadOnly] 
        public NativeArray<float4> InputArray;
        
        [BurstCompile]
        public void Execute(ref ElementComponent elementComponent)
        {
            float3 pos = InputArray[elementComponent.TransformPositionIndex].xyz;
            quaternion rot = quaternion.RotateZ(math.radians(InputArray[elementComponent.TransformRotationIndex].x));
            float3 scl = InputArray[elementComponent.TransformScaleIndex].xyz * elementComponent.Object2DSize.xyx;
            elementComponent.TransformMatrix = float4x4.TRS(pos, rot, scl);
        }
    }
}
