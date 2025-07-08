using System;
using Unity.Burst;
using System.Runtime.InteropServices;
using NUnit.Framework.Constraints;

namespace MNP.Core.Misc
{
    [BurstCompile]
    public struct UnmanagedTimer : IDisposable
    {
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32.dll")]
        public static extern uint GetTickCount();

        private bool _initialized;

        private long m_StartTime;
        private long m_StopTime;
        private long m_Frequency;
        private byte m_IsRunning;

        private bool _useRegularTick;

        [BurstCompile]
        public void Initialize()
        {
            if (!QueryPerformanceFrequency(out m_Frequency))
                _useRegularTick = true;

            _initialized = true;
        }

        [BurstCompile]
        public void Start()
        {
            if (!_initialized)
                throw new InvalidOperationException("使用计时器前需初始化");

            if (_useRegularTick)
            {
                m_StartTime = GetTickCount();
            }
            else
            {
                QueryPerformanceCounter(out m_StartTime);
            }
            m_IsRunning = 1;
        }

        [BurstCompile]
        public void Stop()
        {
            if (!_initialized)
                throw new InvalidOperationException("使用计时器前需初始化");

            if (_useRegularTick)
            {
                m_StopTime = GetTickCount();
            }
            else
            {
                QueryPerformanceCounter(out m_StopTime);
            }
            m_IsRunning = 0;
        }

        public void Reset()
        {
            if (!_initialized)
                throw new InvalidOperationException("使用计时器前需初始化");

            m_StartTime = 0;
            m_StopTime = 0;
            m_IsRunning = 0;
        }

        [BurstCompile]
        public float GetElapsedSeconds()
        {
            if (!_initialized)
                throw new InvalidOperationException("使用计时器前需初始化");

            long elapsed;

            if (m_IsRunning == 1)
            {
                QueryPerformanceCounter(out long currentTime);
                elapsed = currentTime - m_StartTime;
            }
            else
            {
                elapsed = m_StopTime - m_StartTime;
            }

            return (float)elapsed / m_Frequency;
        }

        public void Dispose()
        {
            if (!_initialized)
                throw new InvalidOperationException("使用计时器前需初始化");

            Reset();
            _useRegularTick = false;
            _initialized = false;
        }
    }
}