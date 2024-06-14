using UnityEngine;

public class YourProjectStartup : MonoBehaviour
{
    [SerializeField]
    private Canvas[] canvases;
    
    void Start()
    {
        int i = 0;
        foreach (var display in Display.displays)
        {
            display.Activate();
            canvases[i].targetDisplay = i++;
        }
    }
}
