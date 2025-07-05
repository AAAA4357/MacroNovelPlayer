using Unity.Burst;
using Unity.Collections;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct BezierCurve
    {
        public int Dimension;

        public NativeArray<float> ControlPointP0;

        public NativeArray<float> ControlPointP1;

        public NativeArray<float> ControlPointP2;

        public NativeArray<float> ControlPointP3;
    }
}
