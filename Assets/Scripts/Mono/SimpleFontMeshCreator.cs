using UnityEngine;
using TMPro;

public class SimpleFontMeshCreator : MonoBehaviour
{
    public TMP_FontAsset fontAsset;
    public string characters = "ABCD";

    public Material material;
    
    public void CreateSimpleFontMesh()
    {
        TMP_Character character = fontAsset.characterLookupTable['A'];

        // 转换为标准化 UV 坐标（0 到 1 范围）
        float uvX = character.glyph.glyphRect.x;
        float uvY = character.glyph.glyphRect.y;
        float uvWidth = character.glyph.glyphRect.width;
        float uvHeight = character.glyph.glyphRect.height;

        Debug.Log($"字符 'A' 的 UV 信息：");
        Debug.Log($"UV 左下角: ({uvX}, {uvY})");
        Debug.Log($"UV 大小: ({uvWidth}, {uvHeight})");
    }
}