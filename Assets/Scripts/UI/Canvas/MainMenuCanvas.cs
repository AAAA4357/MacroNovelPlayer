using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : CanvasBase
{
    public TextMeshProUGUI Title;
    public Button Load;
    public Button Setting;
    public Button Exit;

    public override void EnterCanvas()
    {
        float sizeY = Title.GetComponent<RectTransform>().sizeDelta.y;
        DOTween.To(() => Title.GetComponent<RectTransform>().sizeDelta.x, x => Title.GetComponent<RectTransform>().sizeDelta = new(x, sizeY), 1200, 1f).SetEase(Ease.OutCubic);
        Load.transform.DOMoveX(80, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.2f);
        Setting.transform.DOMoveX(330, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.1f);
        Exit.transform.DOMoveX(580, 0.5f).SetEase(Ease.OutQuad);
    }

    public override void ExitCanvas()
    {
        float sizeY = Title.GetComponent<RectTransform>().sizeDelta.y;
        DOTween.To(() => Title.GetComponent<RectTransform>().sizeDelta.x, x => Title.GetComponent<RectTransform>().sizeDelta = new(x, sizeY), 0, 1f).SetEase(Ease.OutCubic);
        Load.transform.DOMoveX(-780, 0.5f).SetEase(Ease.InQuad);
        Setting.transform.DOMoveX(-530, 0.5f).SetEase(Ease.InQuad).SetDelay(0.1f);
        Exit.transform.DOMoveX(-280, 0.5f).SetEase(Ease.InQuad).SetDelay(0.2f);
    }
}
