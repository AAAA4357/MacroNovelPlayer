using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadList : MonoBehaviour
{
    public TextMeshProUGUI NoProject;
    public GameObject ListItem;
    public LoadCanvas LoadCanvas;
    public Transform Content;

    int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            if (value == -1)
            {
                LoadCanvas.Load.GetComponent<Button>().enabled = false;
            }
            else
            {
                LoadCanvas.Load.GetComponent<Button>().enabled = true;
            }
        }
    }

    public void ScanFolder()
    {
        SelectedIndex = -1;
        string exeDirectory = Path.GetDirectoryName(Application.dataPath);
        string projectPath = Path.Combine(exeDirectory, "Projects");
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(0));
        }
        bool noProject = true;
        int index = 0;
        foreach (string path in Directory.EnumerateDirectories(projectPath))
        {
            if (!File.Exists(path + "main.json"))
            {
                continue;
            }
            Instantiate(ListItem, Content);
            ListItem.GetComponent<TextMeshProUGUI>().text = Path.GetDirectoryName(path);
            ListItem.GetComponent<ListItem>().LoadList = this;
            ListItem.GetComponent<ListItem>().Index = index;
            noProject = false;
            index++;
        }
        NoProject.gameObject.SetActive(noProject);
    }
}
