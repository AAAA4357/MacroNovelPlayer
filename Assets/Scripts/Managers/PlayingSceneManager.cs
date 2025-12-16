using System.Collections;
using MNP.Core;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class PlayingSceneManager : MonoBehaviour
{
    public GameObject TextInstance;

    [DisableInPlayMode]
    [DisableInEditorMode]
    public UIState state;

    [Title("各Canvas")]
    public MainMenuCanvas MainCanvas;
    public LoadCanvas LoadCanvas;
    public LoadingCanvas LoadingCanvas;
    public SettingCanvas SettingCanvas;

    SceneLoader loader;

    void Start()
    {
        loader = new()
        {
            ProgressBar = LoadingCanvas.ProgressBar,
            TextInstance = TextInstance
        };
        MainCanvas.EnterCanvas();
    }

    IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(1);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnMenuLoadClick()
    {
        state = UIState.Load;
        MainCanvas.ExitCanvas();
        LoadCanvas.EnterCanvas();
        LoadCanvas.LoadList.ScanFolder();
    }

    public void OnMenuSettingClick()
    {
        state = UIState.Setting;
        MainCanvas.ExitCanvas();
        SettingCanvas.EnterCanvas();
    }

    public void OnMenuExitClick()
    {
        MainCanvas.ExitCanvas();
        StartCoroutine(DelayExit());
    }

    public void OnLoadLoadClick()
    {
        
    }

    public void OnLoadBackClick()
    {
        state = UIState.Menu;
        LoadCanvas.ExitCanvas();
        MainCanvas.EnterCanvas();
    }

    public void OnSettingBackClick()
    {
        state = UIState.Menu;
        SettingCanvas.ExitCanvas();
        MainCanvas.EnterCanvas();
    }
}
