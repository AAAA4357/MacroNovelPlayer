using MNP.Core.Mono;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("中断"))
        {
            ((Test)target).Interrupt();
        }
        if (GUILayout.Button("恢复"))
        {
            ((Test)target).Resume();
        }
        EditorGUI.EndDisabledGroup();
    }
}
