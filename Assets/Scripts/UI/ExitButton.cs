using UnityEditor;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void OnClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
