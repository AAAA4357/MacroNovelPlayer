using Sirenix.OdinInspector;
using UnityEngine;

public abstract class CanvasBase : MonoBehaviour
{
    [Button]
    [DisableInEditorMode]
    public abstract void EnterCanvas();

    [Button]
    [DisableInEditorMode]
    public abstract void ExitCanvas();
}
