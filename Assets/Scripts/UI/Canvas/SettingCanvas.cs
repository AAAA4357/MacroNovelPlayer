using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingCanvas : CanvasBase
{
    public TextMeshProUGUI Title;
    public Button Back;

    public override void EnterCanvas()
    {
        float sizeY = Title.GetComponent<RectTransform>().sizeDelta.y;
        DOTween.To(() => Title.GetComponent<RectTransform>().sizeDelta.x, x => Title.GetComponent<RectTransform>().sizeDelta = new(x, sizeY), 400, 1f).SetEase(Ease.OutCubic);
        Back.transform.DOLocalMoveX(880, 0.5f).SetEase(Ease.OutQuad);
    }

    public override void ExitCanvas()
    {
        float sizeY = Title.GetComponent<RectTransform>().sizeDelta.y;
        DOTween.To(() => Title.GetComponent<RectTransform>().sizeDelta.x, x => Title.GetComponent<RectTransform>().sizeDelta = new(x, sizeY), 0, 1f).SetEase(Ease.OutCubic);
        Back.transform.DOLocalMoveX(1260, 0.5f).SetEase(Ease.OutQuad);
    }
}
