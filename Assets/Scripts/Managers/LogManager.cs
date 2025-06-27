using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MNP.Managers
{
    public class LogManager : MonoBehaviour
    {
        public static List<string> messages;

        private void Awake()
        {
            messages = new();

            LogInfo("日志管理器：初始化完毕", this);
        }

        public static void LogInfo(string message, Object context = null)
        {
            Debug.Log(message, context);
            messages.Add(message);
        }

        public static void LogWarn(string message)
        {

        }

        public static void LogError(string message)
        {

        }
    }
}
