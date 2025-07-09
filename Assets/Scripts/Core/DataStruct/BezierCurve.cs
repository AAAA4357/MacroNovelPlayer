using System.Collections.Generic;

namespace MNP.Core.DataStruct
{
    public struct BezierCurve
    {
        public int Dimension;

        public List<float> ControlPointP0;

        public List<float> ControlPointP1;

        public List<float> ControlPointP2;

        public List<float> ControlPointP3;
    }
}
