using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MultiScreenCanvas : MonoBehaviour
{
    [SerializeField]
    private MultiScreenCanvas leftCanvas;
    [SerializeField]
    private MultiScreenCanvas rightCanvas;
    [SerializeField]
    private Transform contentRoot;

    public MultiScreenCanvas LeftCanvas => leftCanvas;
    public MultiScreenCanvas RightCanvas => rightCanvas;
    public Rect Rect { get; private set; } 
    public Transform ContentRoot => contentRoot;

    private void Awake()
    {
        // we can adjust this if we want to apply a safe area  or whatever
        Rect = GetComponent<RectTransform>().rect;

        if (contentRoot == null)
        {
            contentRoot = transform;
        }
    }
    
}
