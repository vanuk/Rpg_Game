using UnityEngine;
using UnityEngine.EventSystems;

public class Trash : MonoBehaviour, IDropHandler
{
    public Inventory_System inventorySystem;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlot != null)
        {
            inventorySystem.RemoveItem(draggedSlot.slotIndex);
        }
    }
}