using MNP.Helpers;
using Unity.Mathematics;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
    float4 p0;
    float4 p1;
    float4 c0;
    float4 c1;

    const float TotalTime = 2;

    float4 Normalize(float4 value)
    {
        float magnitude = Mathf.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z + value.w * value.w);
        return value / magnitude;
    }

    void Start()
    {
        Unity.Mathematics.Random random = new(1);
        p0 = Normalize(random.NextFloat4(0, 1));
        p1 = Normalize(random.NextFloat4(0, 1));
        c0 = Normalize(random.NextFloat4(0, 1));
        c1 = Normalize(random.NextFloat4(0, 1));
        // 创建有明显旋转差异的四元数
        float angle = math.radians(90f); // 90度旋转
        float4 start = new float4(0, 0, 0, 1);
        float4 end = new float4(0, math.sin(angle/2), 0, math.cos(angle/2));
        
        Debug.Log($"Start: {start}");
        Debug.Log($"End: {end}");
        
        // 先验证初始角度
        float initialTheta = QuaternionHelper.GetTheta(start, end, out int inverse);
        Debug.Log($"Initial theta: {initialTheta} radians, {math.degrees(initialTheta)} degrees");
        
        float4 prev = start;
        for (int i = 0; i <= 10; i++)
        {
            float t = i / 10.0f;
            float4 current = PathLerpHelper.SLerp4DLinear(start, end, t);
            
            // 计算相邻帧之间的角度变化
            float angleStep = QuaternionHelper.GetTheta(prev, current, out _);
            Debug.Log($"t: {t}, Angle Step: {angleStep}");
            
            prev = current;
        }
    }

    void Update()
    {
        float t = Time.time % TotalTime;
        float4 rotation = PathLerpHelper.GetBezierPoint4D(p0, c0, c1, p1, t / TotalTime);
        transform.rotation = new(rotation.x, rotation.y, rotation.z, rotation.w);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.zero, transform.rotation * Vector3.up);
    }
}
