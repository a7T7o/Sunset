using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 运行时补齐精力条 tooltip 入口，避免为了 hover 文案去改 Scene/Prefab。
/// </summary>
[DisallowMultipleComponent]
public class EnergyBarTooltipWatcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EnergySystem energySystem;
    private bool isHovering;

    public void Bind(EnergySystem system)
    {
        energySystem = system;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        EnergySystem system = energySystem != null ? energySystem : EnergySystem.Instance;
        if (system == null)
        {
            return;
        }

        ItemTooltip.Instance?.ShowCustom("精力", $"当前精力: {system.CurrentEnergy}/{system.MaxEnergy}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        ItemTooltip.Instance?.Hide();
    }

    private void OnDisable()
    {
        if (isHovering)
        {
            ItemTooltip.Instance?.Hide();
        }
    }
}
