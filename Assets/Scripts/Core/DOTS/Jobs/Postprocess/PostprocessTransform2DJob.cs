using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(BakeReadyComponent))]
    public partial struct PostprocessTransform2DJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float4> InputArray;
        
        [BurstCompile]
        public void Execute(ref ElementComponent elementComponent)
        {
            if (elementComponent.ObjectType != ObjectType.Object2D)
            {
                return;
            }
            float3 pos = InputArray[elementComponent.TransformPositionIndex].xyz;
            quaternion rot = quaternion.RotateZ(math.radians(InputArray[elementComponent.TransformRotationIndex].x));
            float3 scl = InputArray[elementComponent.TransformScaleIndex].xyz;
            elementComponent.TransformMatrix = float4x4.TRS(pos, rot, scl);
        }
    }
}
