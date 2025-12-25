using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FarmGame.Data;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    [SerializeField] private Image selectedOverlay;

    private InventoryService inventory;
    private EquipmentService equipment;
    private ItemDatabase database;
    private int index;
    private bool isHotbar;

    private float lastClickTime = -1f;
    private const float DoubleClickThreshold = 0.25f;

    void Awake()
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (iconImage == null)
        {
            var t = transform.Find("Icon");
            if (t)
            {
                iconImage = t.GetComponent<Image>();
            }
            else
            {
                // 自动创建 Icon
                var go = new GameObject("Icon");
                go.transform.SetParent(transform, false);
                iconImage = go.AddComponent<Image>();
                iconImage.raycastTarget = false;
                var rt = (RectTransform)iconImage.transform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                iconImage.enabled = false;
            }
        }
        if (amountText == null)
        {
            var t = transform.Find("Amount");
            if (t)
            {
                amountText = t.GetComponent<Text>();
            }
            else
            {
                // 自动创建 Amount
                var go = new GameObject("Amount");
                go.transform.SetParent(transform, false);
                amountText = go.AddComponent<Text>();
                amountText.raycastTarget = false;
                amountText.alignment = TextAnchor.LowerRight;
                amountText.fontSize = 14;
                amountText.color = Color.white;
                amountText.text = "";
                var rt = (RectTransform)amountText.transform;
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(1, 0.5f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }
        if (selectedOverlay == null)
        {
            var t = transform.Find("Selected");
            if (t) selectedOverlay = t.GetComponent<Image>();
        }
    }

    void OnEnable()
    {
        if (inventory != null) inventory.OnSlotChanged += HandleSlotChanged;
        Refresh();
    }

    void OnDisable()
    {
        if (inventory != null) inventory.OnSlotChanged -= HandleSlotChanged;
    }

    public void Bind(InventoryService inv, EquipmentService equip, ItemDatabase db, int slotIndex, bool hotbar)
    {
        inventory = inv;
        equipment = equip;
        database = db;
        index = slotIndex;
        isHotbar = hotbar;
        if (isActiveAndEnabled)
        {
            if (inventory != null)
            {
                inventory.OnSlotChanged -= HandleSlotChanged;
                inventory.OnSlotChanged += HandleSlotChanged;
            }
            Refresh();
        }
    }

    void HandleSlotChanged(int changedIndex)
    {
        if (changedIndex == index) Refresh();
    }

    public void Refresh()
    {
        if (inventory == null || database == null) return;
        var s = inventory.GetSlot(index);
        if (s.IsEmpty)
        {
            // 使用统一的缩放适配工具清除图标
            if (iconImage) UIItemIconScaler.SetIconWithAutoScale(iconImage, null);
            if (amountText) amountText.text = "";
        }
        else
        {
            var data = database.GetItemByID(s.itemId);
            // ✅ 使用统一的缩放适配工具设置图标（自动等比例缩放适配56x56显示区域）
            if (iconImage)
            {
                UIItemIconScaler.SetIconWithAutoScale(iconImage, data != null ? data.GetBagSprite() : null);
            }
            if (amountText)
            {
                amountText.text = s.amount > 1 ? s.amount.ToString() : "";
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        float now = Time.unscaledTime;
        if (now - lastClickTime <= DoubleClickThreshold)
        {
            TryEquipOnDoubleClick();
            lastClickTime = -1f;
        }
        else
        {
            lastClickTime = now;
        }
    }

    void TryEquipOnDoubleClick()
    {
        if (inventory == null || equipment == null) return;
        var s = inventory.GetSlot(index);
        if (s.IsEmpty) return;
        if (!equipment.IsEquipableItemPublic(s.itemId)) return;
        equipment.TryEquipFromInventory(inventory, index, -1);
    }
}
