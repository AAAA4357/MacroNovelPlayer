using System;
using UnityEngine;

namespace MNP.Core.DataStruct
{
    [Serializable]
    public class BezierCurve : IEquatable<BezierCurve>
    {
        /// <summary>
        /// 直线构造函数
        /// </summary>
        /// <param name="p0">起始点</param>
        /// <param name="p1">结束点</param>
        public BezierCurve(Vector2 p0, Vector2 p1)
        {
            P0 = p0;
            P1 = p0 + (p1 - p0) / 3f;
            P2 = p0 + (p1 - p0) / 1.5f;
            P3 = p1;
        }

        /// <summary>
        /// 圆弧构造函数
        /// </summary>
        /// <param name="p0">起始点</param>
        /// <param name="p1">结束点</param>
        /// <param name="radius">半径（为正则取连线左侧，为负则取连线右侧）</param>
        public BezierCurve(Vector2 p0, Vector2 p1, float radius)
        {
            //中点
            Vector2 mid = (p0 + p1) / 2;
            float d = (p1 - p0).magnitude;

            //方向
            Vector2 dir = (p1 - p0).normalized;
            Vector2 dir1 = -Mathf.Sign(radius) * Vector2.Perpendicular(dir);
            float dm = Mathf.Sqrt(radius * radius - d * d / 4);

            //圆心
            Vector2 rCenter = mid + dm * dir1;

            //角度θ
            float a1 = -Vector2.SignedAngle(p0 - rCenter, p1 - rCenter) * Mathf.PI / 180f;
            float a2 = -Vector2.SignedAngle(Vector2.right, p1 - rCenter) * Mathf.PI / 180f;

            //负值修正
            if (radius < 0)
                a2 += Mathf.PI;

            float halfSina = Mathf.Sin(a1 / 2f);
            float halfCosa = Mathf.Cos(a1 / 2f);

            float h = (4f - 4f * halfCosa) / (3f * halfSina);

            //控制点
            Vector2 c1 = new Vector2(1, h) * radius + rCenter;
            Vector2 c2 = new Vector2(Mathf.Cos(a1) + h * Mathf.Sin(a1), Mathf.Sin(a1) - h * Mathf.Cos(a1)) * radius + rCenter;
            
            //控制点变换
            float fc1x = (c1.x - rCenter.x) * Mathf.Cos(a2) + (c1.y - rCenter.y) * Mathf.Sin(a2) + rCenter.x;
            float fc1y = -(c1.x - rCenter.x) * Mathf.Sin(a2) + (c1.y - rCenter.y) * Mathf.Cos(a2) + rCenter.y;
            float fc2x = (c2.x - rCenter.x) * Mathf.Cos(a2) + (c2.y - rCenter.y) * Mathf.Sin(a2) + rCenter.x;
            float fc2y = -(c2.x - rCenter.x) * Mathf.Sin(a2) + (c2.y - rCenter.y) * Mathf.Cos(a2) + rCenter.y;

            //修正控制点
            Vector2 fc1 = new(fc2x, fc2y);
            Vector2 fc2 = new(fc1x, fc1y);

            P0 = p0;
            P1 = fc1;
            P2 = fc2;
            P3 = p1;
        }

        /// <summary>
        /// 自定义构造函数
        /// </summary>
        /// <param name="p0">起始点</param>
        /// <param name="p1">控制点1</param>
        /// <param name="p2">控制点2</param>
        /// <param name="p3">结束点</param>
        public BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        /// <summary>
        /// 起始点
        /// </summary>
        public Vector2 P0;

        /// <summary>
        /// 控制点1
        /// </summary>
        public Vector2 P1;

        /// <summary>
        /// 控制点2
        /// </summary>
        public Vector2 P2;

        /// <summary>
        /// 结束点
        /// </summary>
        public Vector2 P3;

        public bool Equals(BezierCurve other)
        {
            return P0 == other.P0 && P1 == other.P1 && P2 == other.P2 && P3 == other.P3;
        }
    }
}
