using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FarmGame.Data;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;

    private EquipmentService equipment;
    private InventoryService inventory;
    private ItemDatabase database;
    private int index; // 0..5

    void Awake()
    {
        if (iconImage == null)
        {
            var t = transform.Find("Icon");
            if (t) iconImage = t.GetComponent<Image>();
        }
        if (amountText == null)
        {
            var t = transform.Find("Amount");
            if (t) amountText = t.GetComponent<Text>();
        }
    }

    void OnEnable()
    {
        if (equipment != null) equipment.OnEquipSlotChanged += HandleEquipChanged;
        Refresh();
    }

    void OnDisable()
    {
        if (equipment != null) equipment.OnEquipSlotChanged -= HandleEquipChanged;
    }

    public void Bind(EquipmentService equip, InventoryService inv, ItemDatabase db, int equipIndex)
    {
        equipment = equip;
        inventory = inv;
        database = db;
        index = equipIndex;
        if (isActiveAndEnabled)
        {
            OnDisable();
            OnEnable();
        }
        else
        {
            Refresh();
        }
    }

    void HandleEquipChanged(int changed)
    {
        if (changed == index) Refresh();
    }

    public void Refresh()
    {
        if (equipment == null || database == null) return;
        var s = equipment.GetEquip(index);
        if (s.IsEmpty)
        {
            // 使用统一的缩放适配工具清除图标
            if (iconImage) UIItemIconScaler.SetIconWithAutoScale(iconImage, null);
            if (amountText) amountText.text = "";
            return;
        }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            equipment?.UnequipToInventory(inventory, index);
        }
    }
}
