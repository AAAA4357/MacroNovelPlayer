using System;
using System.Diagnostics;
using UnityEngine;

namespace MNP.Managers
{
    public class TimeManager : MonoBehaviour
    {
        /// <summary>
        /// 实际时间戳（以毫秒为单位）
        /// </summary>
        public long RealTimeStamp { get; set; } = 0;

        /// <summary>
        /// 游戏时间戳（以毫秒为单位）
        /// </summary>
        public long GameTimeStamp { get; set; } = 0;

        /// <summary>
        /// 计时缩放因子（时间倍数）,默认为1
        /// </summary>
        public float TimeScaleFactor { get; set; } = 1;

        /// <summary>
        /// 计时启用
        /// </summary>
        public bool IsTimeEnabled { get; set; } = false;

        /// <summary>
        /// 游戏时间暂停
        /// </summary>
        public bool IsGameTimeStampPaused { get; set; } = false;


        Stopwatch mainTimer;

        private void OnEnable()
        {
            mainTimer = new();
            mainTimer.Reset();
        }

        private void Update()
        {
            if (!IsTimeEnabled)
                return;
            if (!mainTimer.IsRunning)
            {
                mainTimer.Start();
                return;
            }
            mainTimer.Stop();
            long fixedElapsedTime = (long)((mainTimer.ElapsedMilliseconds << 8) * TimeScaleFactor) >> 8;
            if (!IsGameTimeStampPaused)
            {
                GameTimeStamp += fixedElapsedTime;
            }
            RealTimeStamp += fixedElapsedTime;
            mainTimer.Restart();
        }

        private void OnDisable()
        {
            mainTimer.Stop();
            mainTimer = null;
        }

        /// <summary>
        /// 计时开始（如初始值不为零则继续计时），包括游戏时间及实际时间
        /// </summary>
        public void TimeStart()
        {
            IsTimeEnabled = true;
            mainTimer?.Stop();
            mainTimer?.Reset();
        }

        /// <summary>
        /// 计时停止，包括游戏时间及实际时间
        /// </summary>
        public void TimeStop()
        {
            IsTimeEnabled = false;
            mainTimer?.Stop();
            mainTimer?.Reset();
        }

        /// <summary>
        /// 计时状态切换，包括游戏时间及实际时间
        /// </summary>
        public void TimeToggle()
        {
            IsTimeEnabled = !IsTimeEnabled;
            mainTimer?.Stop();
            mainTimer?.Reset();
        }

        /// <summary>
        /// 计时重置，包括游戏时间及实际时间
        /// </summary>
        public void TimeReset()
        {
            GameTimeStamp = 0;
            RealTimeStamp = 0;
            mainTimer?.Stop();
            mainTimer?.Reset();
        }

        /// <summary>
        /// 游戏时间计时暂停
        /// </summary>
        public void GameTimePause()
        {
            IsGameTimeStampPaused = true;
        }

        /// <summary>
        /// 游戏时间计时恢复
        /// </summary>
        public void GameTimeResume()
        {
            IsGameTimeStampPaused = false;
        }

        /// <summary>
        /// 游戏时间计时状态切换
        /// </summary>
        public void GameTimeToggle()
        {
            IsGameTimeStampPaused = !IsGameTimeStampPaused;
        }

        /// <summary>
        /// 计时中立即停止并重新开始计时，包括游戏时间及实际时间
        /// </summary>
        public void TimeRestart()
        {
            TimeStop();
            TimeReset();
            TimeStart();
        }

        /// <summary>
        /// 设置游戏时间戳（以毫秒为单位）
        /// </summary>
        public void SetGameTimeStamp(long timeStamp)
        {
            GameTimeStamp = timeStamp;
        }

        /// <summary>
        /// 设置实际时间戳（以毫秒为单位）
        /// </summary>
        public void SetRealTimeStamp(long timeStamp)
        {
            RealTimeStamp = timeStamp;
        }

        /// <summary>
        /// 设置计时缩放因子
        /// </summary>
        /// <param name="scale">缩放因子</param>
        public void SetTimeScaleFactor(float scale)
        {
            TimeScaleFactor = scale;
        }
    }
}
