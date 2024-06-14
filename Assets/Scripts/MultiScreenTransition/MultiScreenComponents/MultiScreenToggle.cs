using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiScreen
{
    [RequireComponent(typeof(Toggle))]
    public class MultiScreenToggle : BaseMultiScreenComponent<Toggle>, IPointerClickHandler, ISubmitHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(DelayedMirror());
        }

        public void OnSubmit(BaseEventData eventData)
        {
            StartCoroutine(DelayedMirror());
        }

        private IEnumerator DelayedMirror()
        {
            // We can't be sure which OnPointerClick is called first: this, or the Toggle's. 
            // So we wait to make sure the Toggle's was called and the correct value was set, 
            // and only mirror it on the copy afterwards
            yield return null;
            if (copy != null)
            {
                copy.Component.SetIsOnWithoutNotify(Component.isOn);
            }

            // Note: We do not want to attach to the Toggle callback directly, since the user's code might remove listeners
        }
    }
}
