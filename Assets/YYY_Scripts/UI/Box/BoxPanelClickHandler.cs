using UnityEngine;
using UnityEngine.EventSystems;

namespace FarmGame.UI
{
    /// <summary>
    /// 箱子面板空白区点击处理器。
    /// 对齐背包面板的“空白区点击 = 回源 / 垃圾桶 = 丢弃”语义。
    /// </summary>
    public class BoxPanelClickHandler : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        [SerializeField] private bool isDropZone;
        private BoxPanelUI owner;

        public void Configure(BoxPanelUI panel, bool dropZone)
        {
            owner = panel;
            isDropZone = dropZone;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || owner == null)
            {
                return;
            }

            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject ?? eventData.pointerPressRaycast.gameObject;
            if (clickedObject != gameObject)
            {
                return;
            }

            owner.HandleHeldClickOutside(eventData.position, isDropZone);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!isDropZone || owner == null)
            {
                return;
            }

            owner.HandleHeldClickOutside(eventData.position, true);
        }
    }
}
