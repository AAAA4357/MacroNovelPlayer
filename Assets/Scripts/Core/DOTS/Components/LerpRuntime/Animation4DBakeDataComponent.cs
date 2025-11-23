using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct Animation4DBakeDataComponent : IBufferElementData
    {
        public float4 q0;       //q0
        public float4 q01;      //q01
        public float4 q01_1q12; //q01^-1*q12
        public float4 q12_1q23; //q12^-1*q23
    }
}
