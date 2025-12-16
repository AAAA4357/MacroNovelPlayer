using UnityEngine;

public class ListItem : MonoBehaviour
{
    public LoadList LoadList;
    public int Index;

    public void OnSelect()
    {
        if (LoadList.SelectedIndex == Index)
        {
            LoadList.SelectedIndex = -1;
        }
        else
        {
            LoadList.SelectedIndex = Index;
        }
    }
}
