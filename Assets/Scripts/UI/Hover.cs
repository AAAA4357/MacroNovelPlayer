using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Hover : MonoBehaviour
{
    void OnMouseEnter()
    {
        GetComponent<CanvasGroup>().alpha = 0.6f;
    }

    void OnMouseExit()
    {
        GetComponent<CanvasGroup>().alpha = 1f;
    }
}
