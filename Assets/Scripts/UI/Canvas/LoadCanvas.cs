using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadCanvas : CanvasBase
{
    public TextMeshProUGUI Title;
    public Button Load;
    public Button Back;

    public override void EnterCanvas()
    {
        float sizeY = Title.GetComponent<RectTransform>().sizeDelta.y;
        DOTween.To(() => Title.GetComponent<RectTransform>().sizeDelta.x, x => Title.GetComponent<RectTransform>().sizeDelta = new(x, sizeY), 500, 1f).SetEase(Ease.OutCubic);
        Load.transform.DOLocalMoveX(630, 0.5f).SetEase(Ease.OutQuad);
        Back.transform.DOLocalMoveX(880, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.1f);
    }

    public override void ExitCanvas()
    {
        float sizeY = Title.GetComponent<RectTransform>().sizeDelta.y;
        DOTween.To(() => Title.GetComponent<RectTransform>().sizeDelta.x, x => Title.GetComponent<RectTransform>().sizeDelta = new(x, sizeY), 0, 1f).SetEase(Ease.OutCubic);
        Load.transform.DOLocalMoveX(1260, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.1f);
        Back.transform.DOLocalMoveX(1510, 0.5f).SetEase(Ease.OutQuad);
    }
}
