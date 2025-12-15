using MNP.Core;
using MNP.Mono;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public Button Load;
    public LoadProgressBar bar;
    public Test test;

    public void OnButtonClick()
    {
        SceneLoader loader = new()
        {
            Bar = bar.Bar,
            canvas = bar.canvas,
            TextInstance = test.TextInstance
        };
        _ = loader.LoadProject(test.TestProject);
    }
}
