using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleFontMeshCreator))]
public class SimpleFontMeshCreatorEditor : Editor
{
    bool created;
    int index;

    Mesh mesh;

    void OnEnable()
    {
        created = false;
        mesh = null;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginDisabledGroup(created);
        if (GUILayout.Button("生成Mesh"))
        {
            TMP_FontAsset asset = ((SimpleFontMeshCreator)target).fontAsset;
            foreach (char c in ((SimpleFontMeshCreator)target).characters)
            {
                asset.characterLookupTable.TryGetValue(c, out var character);

                // 转换为标准化 UV 坐标（0 到 1 范围）
                float uvX = (float)character.glyph.glyphRect.x / asset.atlasWidth;
                float uvY = (float)character.glyph.glyphRect.y / asset.atlasHeight;
                float uvWidth = (float)character.glyph.glyphRect.width / asset.atlasWidth;
                float uvHeight = (float)character.glyph.glyphRect.height / asset.atlasHeight;

                Debug.Log($"字符 {c} 的 UV 信息：");
                Debug.Log($"UV 坐标: ({uvX}, {uvY})");
                Debug.Log($"UV 大小: ({uvWidth}, {uvHeight})");
            }
        }
        EditorGUI.EndDisabledGroup();
        if (created)
        {
            int count = mesh.subMeshCount;
            index = EditorGUILayout.IntSlider(index, 0, count - 1);
        }
    }

    void OnSceneGUI()
    {
        Graphics.DrawMesh(mesh, Matrix4x4.identity, ((SimpleFontMeshCreator)target).material, 0, Camera.main, index);
    }
}
