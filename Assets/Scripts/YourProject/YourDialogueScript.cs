using System;
using UnityEngine;
using UnityEngine.UI;

namespace MultiScreen
{
    public class YourDialogueScript : MonoBehaviour
    {
        [SerializeField]
        private Button importantButton;

        [SerializeField]
        private Toggle essentialToggle;

        private void Start()
        {
            importantButton.onClick.AddListener(() =>
            {
                Debug.Log("The important button has been clicked");
            });

            essentialToggle.onValueChanged.AddListener((x) =>
            {
                Debug.Log("The essential toggle has been switched to " + x);
            }); 
        }
    }
}
