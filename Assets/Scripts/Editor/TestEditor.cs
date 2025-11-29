using MNP.Mono;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("恢复所有播放"))
        {
            Test test = (Test)target;
            test.ResumePlay();
        }
    }
}
