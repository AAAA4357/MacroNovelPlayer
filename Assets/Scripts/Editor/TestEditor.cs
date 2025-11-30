using MNP.Mono;
using MNP.Core.DOTS.Systems;
using Unity.Entities;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    bool isPlaying;
    AnimBool enterPlaying;
    AnimBool exitPlaying;

    float resetTime;

    void OnEnable()
    {
        enterPlaying = new();
        exitPlaying = new();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        enterPlaying.target = Application.isPlaying;
        exitPlaying.target = !Application.isPlaying;    

        if (EditorGUILayout.BeginFadeGroup(exitPlaying.faded))
        {
            EditorGUILayout.HelpBox("调试工具仅可在启用播放后使用", MessageType.Warning);
        }
        EditorGUILayout.EndFadeGroup();
        if (EditorGUILayout.BeginFadeGroup(enterPlaying.faded))
        {
            if (Application.isPlaying)
            {
                SystemHandle handle = World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingUnmanagedSystem<TimeSystem>();
                ref TimeSystem timeSystem = ref World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<TimeSystem>(handle);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField("动画时间：", timeSystem.SystemTime);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(isPlaying);
                if (GUILayout.Button("开始播放"))
                {
                    Test test = (Test)target;
                    test.EnableTime();
                    isPlaying = true;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(!isPlaying);
                if (GUILayout.Button("停止播放"))
                {
                    Test test = (Test)target;
                    test.DisableTime();
                    isPlaying = false;
                }
                if (GUILayout.Button("中断所有播放"))
                {
                    Test test = (Test)target;
                    test.InterruptPlay();
                }
                if (GUILayout.Button("恢复所有播放"))
                {
                    Test test = (Test)target;
                    test.ResumePlay();
                }
                if (GUILayout.Button("重置时间"))
                {
                    timeSystem.SystemTime = 0;
                }
                resetTime = EditorGUILayout.FloatField("目标时间", resetTime);
                if (GUILayout.Button("重设"))
                {
                    timeSystem.SystemTime = resetTime;
                }
                EditorGUI.EndDisabledGroup();
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorUtility.SetDirty(target);
    }
}
