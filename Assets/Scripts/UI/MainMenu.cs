using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI Title;

    public Button Load;
    public Button Exit;

    void Start()
    {
        Title.transform.DOMoveX(160, 1);
        Load.transform.DOMoveX(160, 1).SetDelay(0.2f);
        Exit.transform.DOMoveX(160, 1).SetDelay(0.4f);
    }
}
