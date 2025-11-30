using MNP.Mono;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    bool isPlaying;
    AnimBool enterPlaying;
    AnimBool exitPlaying;

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
            if (GUILayout.Button("开始播放"))
            {
                Test test = (Test)target;
                test.EnableTime();
                isPlaying = true;
            }
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
                test.ResumePlay();
            }
            if (GUILayout.Button("恢复所有播放"))
            {
                Test test = (Test)target;
                test.ResumePlay();
            }
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndFadeGroup();
    }
}
