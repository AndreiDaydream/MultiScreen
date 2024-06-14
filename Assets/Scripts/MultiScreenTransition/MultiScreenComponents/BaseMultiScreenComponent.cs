using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiScreen
{
    [RequireComponent(typeof(Selectable))]
    public class BaseMultiScreenComponent<T> : MonoBehaviour, 
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
        where T : Selectable
    {
        public T Component { get; private set; }

        protected BaseMultiScreenComponent<T> copy;

        private void Awake()
        {
            Component = GetComponent<T>();
        }

        public void SetCopy(BaseMultiScreenComponent<T> copy)
        {
            this.copy = copy;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            copy.Component.OnPointerDown(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            copy.Component.OnPointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            copy.Component.OnPointerExit(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            copy.Component.OnSelect(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            copy.Component.OnDeselect(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            copy.Component.OnPointerUp(eventData);
        }
    }
}
