using Unity.Mathematics;

public static class QuaternionHelper
{
    public static float4 Inverse(this float4 value)
    {
        return new(value.x, -value.yzw);
    }

    public static float4 Log(this float4 value)
    {
        float magnitude = math.sqrt(value.y * value.y + value.z * value.z + value.w * value.w);
        return new(0, value.yzw / magnitude * math.acos(value.x));
    }

    public static float4 Exp(this float4 value)
    {
        float magnitude = math.sqrt(value.y * value.y + value.z * value.z + value.w * value.w);
        return new(math.cos(magnitude), value.yzw / magnitude * math.sin(magnitude));
    }

    public static float4 Pow(this float4 value, float power)
    {
        return (value.Log() * power).Exp();
    }

    public static float4 Mul(float4 lhs, float4 rhs)
    {
        return new(lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z - lhs.w * rhs.w,
                   lhs.x * rhs.y + rhs.x * lhs.y + lhs.z * rhs.w - rhs.z * lhs.w,
                   lhs.x * rhs.z + rhs.x * lhs.z + rhs.y * lhs.w - lhs.y * rhs.w,
                   lhs.x * rhs.w + rhs.x * lhs.w + lhs.y * rhs.z - rhs.y * lhs.z);
    }

    public static float GetTheta(float4 start, float4 end, out int inverse)
    {
        float dot = DotMul(start, end);
        if (dot < 0) 
        {
            dot = -dot;
            inverse = -1;
        }
        else
        {
            inverse = 1;
        }
        dot = math.clamp(dot, -1, 1);
        return 2 * math.acos(dot);
    }

    public static float DotMul(float4 lhs, float4 rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w;
    }
}
