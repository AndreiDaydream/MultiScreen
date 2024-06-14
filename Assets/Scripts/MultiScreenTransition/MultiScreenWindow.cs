
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiScreen
{
    [RequireComponent(typeof(RectTransform))]
    public class MultiScreenWindow : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        private float Width => rectTransform.rect.width * rectTransform.localScale.x;

        private float anchorX;
        private Vector2 anchorOffset;
        private RectTransform rectTransform;
        private MultiScreenCanvas canvas;

        private MultiScreenWindow copy;
        private float screenSizeDelta;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<MultiScreenCanvas>();
            anchorX = (rectTransform.anchorMin.x + rectTransform.anchorMax.x) / 2;
        }

        private void Start()
        {
            anchorOffset.x = anchorX * canvas.Rect.width;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Make object be always on top in its container, ensuring no weird ordering bugs when overlapping strangely with a differently ordered object from another canvas
            transform.SetAsLastSibling();

            if (copy != null)
            {
                copy.transform.SetAsLastSibling();
            }

            // Note: The user might want to move the current dialogue on top when clicked, not just when dragged, but we leave that up to the user
            // in this code, we only really need this for the drag part to work.
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta;

            Vector2 copyAnchor = new ();
            copyAnchor.y = rectTransform.anchoredPosition.y; // Adjust if we want multiple vertical screens
    
            Vector2 pos = rectTransform.anchoredPosition + anchorOffset;
            
            float offsideRight = pos.x + Width * (1 - rectTransform.pivot.x) - canvas.Rect.width;
            float offsideLeft = pos.x - Width * rectTransform.pivot.x;
            
            if (offsideRight > 0)
            {
                if (copy == null)
                {
                    CreateCopy(Direction.Right);
                }

                if (offsideRight > Width && copy != null)
                {
                    eventData.pointerDrag = copy.gameObject;
                    copy.OnBeginDrag(eventData);
                    copy.OnDrag(eventData);
                    return;
                }

                copyAnchor.x = offsideRight - anchorOffset.x - rectTransform.pivot.x * Width + anchorX * screenSizeDelta;
            }
            else if(offsideLeft < 0)
            {
                if (copy == null)
                {
                    CreateCopy(Direction.Left);
                }

                if (offsideLeft < -Width && copy != null)
                {
                    eventData.pointerDrag = copy.gameObject;
                    copy.OnBeginDrag(eventData);
                    copy.OnDrag(eventData);
                    return;
                }

                copyAnchor.x = offsideLeft + copy.canvas.Rect.width - anchorOffset.x + rectTransform.pivot.x * Width + anchorX * screenSizeDelta;
            }
            // TODO: Handle vertical screens? And the dreaded corner screens?
            else
            {
                KillCopy();
            }

            if (copy != null)
            {
                copy.rectTransform.anchoredPosition = copyAnchor;
            }
        }

        private void CreateCopy(Direction dir)
        {
            MultiScreenCanvas copyCanvas = dir switch
            {
                Direction.Right => canvas.RightCanvas,
                Direction.Left => canvas.LeftCanvas,
                _=> null,
            };
            
            if (copyCanvas == null)
            {
                // Treat this as just an edge of screen, nothing to do here
                return;
            }
            
            // TODO: Handle Toggle Groups when duplicating
            // Currently, if we duplicate an active Toggle, the copy will take the chosen status, making the original appear unselected.
            // A possible solution is to remove the ToggleGroup for the copy; and when a copy is killed, take over its Toggle group, if any
            // so in case the "original" was killed, the toggleGroup is not lost
            // This should be done in a separate component for the Toggle component only
            copy = Instantiate(this, copyCanvas.ContentRoot);
            copy.name = name;
            copy.copy = this;
            
            // TODO: Improve performance
            // To make this very "plug-and-play", we sacrificed performance and can expect a small spike when creating the copy due to many heavy calls.
            // To optimize for performance, we can add these components before the copy is actually needed (either in editor or by code)
            // and just iterate through the existing components and link them when creating the copy
            // Alternatively, we can turn this into a coroutine and spread the setup over multiple frames, 
            // but this ads many corner-cases that need to be treated for interacting with the copy before it is fully initialized
            Selectable[] selectables = GetComponentsInChildren<Selectable>();
            Selectable[] selectableCopies = copy.GetComponentsInChildren<Selectable>();
            for (int i = 0; i < selectables.Length; ++i)
            {
                switch (selectables[i])
                {
                    case Toggle _:
                        var multiScreenToggle = selectables[i].AddComponent<MultiScreenToggle>();
                        var multiScreenToggleCopy = selectableCopies[i].AddComponent<MultiScreenToggle>();
                        multiScreenToggle.SetCopy(multiScreenToggleCopy);
                        multiScreenToggleCopy.SetCopy(multiScreenToggle);
                        break;
                    // TODO: Handle other special cases: Dropdowns, input fields, etc
                    default:
                        var multiScreenSelectable = selectables[i].AddComponent<MultiScreenSelectable>();
                        var multiScreenSelectableCopy = selectableCopies[i].AddComponent<MultiScreenSelectable>();
                        multiScreenSelectable.SetCopy(multiScreenSelectableCopy);
                        multiScreenSelectableCopy.SetCopy(multiScreenSelectable);
                        break;
                }
            }

            screenSizeDelta = canvas.Rect.width - copy.canvas.Rect.width;
            copy.screenSizeDelta = -screenSizeDelta;

            copy.transform.SetAsLastSibling();
        }
        
        private void KillCopy()
        {
            if (copy == null) return;
            
            // TODO: Recycle? Consider possible changes while the copy was disabled
            foreach (var copyComponent in GetComponentsInChildren<MultiScreenSelectable>())
            {
                Destroy(copyComponent);
            }

            foreach (var copyComponent in GetComponentsInChildren<MultiScreenToggle>())
            {
                Destroy(copyComponent);
            }
            Destroy(copy.gameObject);
        }
    }
}
