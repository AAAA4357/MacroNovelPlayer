using MNP.Managers;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor
{
    AnimBool debugEnabled;
    AnimBool debugRealTimeStampWarnEnabled;
    AnimBool debugGameTimeStampWarnEnabled;
    long debugGameTimeValue = 0;
    long debugRealTimeValue = 0;
    float debugScaleFactorValue = 1;

    private void OnEnable()
    {
        debugEnabled = new AnimBool(false);
        debugRealTimeStampWarnEnabled = new AnimBool(false);
        debugGameTimeStampWarnEnabled = new AnimBool(false);
        debugEnabled.valueChanged.AddListener(Repaint);
        debugRealTimeStampWarnEnabled.valueChanged.AddListener(Repaint);
        debugGameTimeStampWarnEnabled.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        GUIStyle h1 = new(GUI.skin.label)
        {
            fontSize = 20
        };
        GUIStyle h2 = new(GUI.skin.label)
        {
            fontSize = 16
        };
        GUILayout.Label("时间管理器", h1);
        EditorGUILayout.Separator();
        GUILayout.Label("管理器属性", h2);
        EditorGUILayout.Separator();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LongField("实际时间戳：", ((TimeManager)target).RealTimeStamp);
        GUILayout.Space(5);
        EditorGUILayout.LongField("游戏时间戳：", ((TimeManager)target).GameTimeStamp);
        GUILayout.Space(5);
        EditorGUILayout.Toggle("计时启用", ((TimeManager)target).IsTimeEnabled);
        GUILayout.Space(5);
        EditorGUILayout.Toggle("游戏时间暂停", ((TimeManager)target).IsGameTimeStampPaused);
        GUILayout.Space(5);
        EditorGUILayout.FloatField("计时缩放因子", ((TimeManager)target).TimeScaleFactor);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Separator();
        debugEnabled.target = EditorGUILayout.ToggleLeft("启用调试操作", debugEnabled.target, h2);
        EditorGUILayout.Separator();
        if (EditorGUILayout.BeginFadeGroup(debugEnabled.faded))
        {
            GUILayout.BeginHorizontal();
            debugRealTimeValue = EditorGUILayout.LongField("实际时间戳：", debugRealTimeValue);
            GUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(debugRealTimeValue < 0);
            if (GUILayout.Button("设置实际游戏时间戳", GUILayout.Height(20)))
            {
                ((TimeManager)target).SetRealTimeStamp(debugRealTimeValue);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            debugRealTimeStampWarnEnabled.target = debugRealTimeValue < 0 ? true : false;
            if (EditorGUILayout.BeginFadeGroup(debugRealTimeStampWarnEnabled.faded))
            {
                EditorGUILayout.HelpBox("警告：实际时间戳不可为负值以免带来不可控因素", MessageType.Warning);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            debugGameTimeValue = EditorGUILayout.LongField("游戏时间戳：", debugGameTimeValue);
            GUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(debugGameTimeValue < 0 | debugGameTimeValue > debugRealTimeValue | debugGameTimeValue > ((TimeManager)target).RealTimeStamp);
            if (GUILayout.Button("设置初始游戏时间戳", GUILayout.Height(20)))
            {
                ((TimeManager)target).SetGameTimeStamp(debugGameTimeValue);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            debugGameTimeStampWarnEnabled.target = (debugGameTimeValue < 0 | debugGameTimeValue > debugRealTimeValue | debugGameTimeValue > ((TimeManager)target).RealTimeStamp) ? true : false;
            if (EditorGUILayout.BeginFadeGroup(debugGameTimeStampWarnEnabled.faded))
            {
                EditorGUILayout.HelpBox("警告：游戏时间戳不可为负值且赋值前后大于实际时间戳以免带来不可控因素", MessageType.Warning);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("切换计时状态", GUILayout.Height(30)))
            {
                ((TimeManager)target).TimeToggle();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("切换游戏时间暂停状态", GUILayout.Height(30)))
            {
                ((TimeManager)target).GameTimeToggle();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            debugScaleFactorValue = EditorGUILayout.Slider("计时缩放因子：", debugScaleFactorValue, 0.1f, 10f);
            GUILayout.Space(5);
            if (GUILayout.Button("设置计时缩放因子", GUILayout.Height(20)))
            {
                ((TimeManager)target).SetTimeScaleFactor(debugScaleFactorValue);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("重置参数", GUILayout.Height(40)))
            {
                debugRealTimeValue = 0;
                debugGameTimeValue = 0;
                debugScaleFactorValue = 1;
                ((TimeManager)target).SetRealTimeStamp(0);
                ((TimeManager)target).SetGameTimeStamp(0);
                ((TimeManager)target).TimeStop();
                ((TimeManager)target).GameTimeResume();
                ((TimeManager)target).SetTimeScaleFactor(1);
            }
        }
        EditorGUILayout.EndFadeGroup();

        this.Repaint();
    }
}
