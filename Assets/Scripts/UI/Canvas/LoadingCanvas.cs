using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LoadingCanvas : CanvasBase
{
    public Slider ProgressBar;
    public TextMeshProUGUI HintText;

    public override void EnterCanvas()
    {
        ProgressBar.transform.DOMoveY(320, 0.5f).SetEase(Ease.OutQuad);
        HintText.transform.DOMoveY(250, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.1f);
    }

    public override void ExitCanvas()
    {
        ProgressBar.transform.DOMoveY(-320, 0.5f).SetEase(Ease.InQuad).SetDelay(0.1f);
        HintText.transform.DOMoveY(-250, 0.5f).SetEase(Ease.InQuad);
    }
}
