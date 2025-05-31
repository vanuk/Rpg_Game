using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public EquipmentType slotType;
    public Inventory_System inventorySystem;
    public Image iconImage;
    public Sprite defaultIconSprite;
    private GameObject dragIcon;
    private Canvas canvas;
    private ItemData equippedItem;
    private int equippedCount;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        SyncFromSystemSlot();
        UpdateIcon();
    }

    private int GetEquipmentIndex()
    {
        switch (slotType)
        {
            case EquipmentType.Weapon: return 0;
            case EquipmentType.Armor: return 1;
            case EquipmentType.Boots: return 2;
            case EquipmentType.Ring: return 3;
            case EquipmentType.Helmet: return 4;
            case EquipmentType.Potion: return 5;
            default: return -1;
        }
    }

    private void SetSystemSlot(ItemData item, int count)
    {
        if (inventorySystem == null) return;
        int idx = GetEquipmentIndex();
        if (idx >= 0 && idx < inventorySystem.equipmentSlots.Count)
        {
            inventorySystem.equipmentSlots[idx].item = item;
            inventorySystem.equipmentSlots[idx].count = count;
        }
    }

    private void RemoveFromEquipmentSlot()
    {
        if (inventorySystem == null) return;
        int idx = GetEquipmentIndex();
        if (idx >= 0 && idx < inventorySystem.equipmentSlots.Count)
        {
            inventorySystem.equipmentSlots[idx].item = null;
            inventorySystem.equipmentSlots[idx].count = 0;
        }
    }

  private void SyncFromSystemSlot()
{
    int idx = GetEquipmentIndex();
    if (inventorySystem != null && idx >= 0 && idx < inventorySystem.equipmentSlots.Count)
    {
        equippedItem = inventorySystem.equipmentSlots[idx].item;
        equippedCount = inventorySystem.equipmentSlots[idx].count;
        Debug.Log($"SyncFromSystemSlot: idx={idx}, equippedItem={(equippedItem != null ? equippedItem.name : "null")}");
    }
}

    private void UpdateIcon()
    {
        if (iconImage == null) return;
        if (equippedItem == null)
        {
            iconImage.sprite = defaultIconSprite;
            iconImage.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            iconImage.sprite = equippedItem.icon;
            iconImage.color = Color.white;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (inventorySystem == null || iconImage == null)
        {
            Debug.LogWarning("EquipmentSlotUI: inventorySystem або iconImage не призначено!");
            return;
        }

        var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlot != null)
        {
            var slot = inventorySystem.GetSlot(draggedSlot.slotIndex);
            if (slot == null)
            {
                Debug.LogWarning("EquipmentSlotUI: slot is null!");
                return;
            }
            if (slot.item != null && slot.item.itemType == slotType)
            {
                equippedItem = slot.item;
                equippedCount = slot.count;
                SetSystemSlot(equippedItem, equippedCount);
                slot.item = null;
                slot.count = 0;
                inventorySystem.inventoryUI.Refresh(inventorySystem._inventorySlots);
                UpdateIcon();
            }
            else if (slot.item != null)
            {
                Debug.LogWarning($"EquipmentSlotUI: itemType mismatch! itemType={slot.item.itemType}, slotType={slotType}");
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
{
    SyncFromSystemSlot(); // Додаємо це!
    if (equippedItem == null) return;
    dragIcon = new GameObject("DragIcon");
    dragIcon.transform.SetParent(canvas.transform, false);
    dragIcon.transform.SetAsLastSibling();
    var image = dragIcon.AddComponent<Image>();
    image.sprite = iconImage.sprite;
    image.raycastTarget = false;
    image.SetNativeSize();
    dragIcon.GetComponent<RectTransform>().sizeDelta = iconImage.rectTransform.sizeDelta;
    dragIcon.transform.position = eventData.position;
    eventData.pointerDrag = this.gameObject;
}

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
    }

public void OnEndDrag(PointerEventData eventData)
{
    if (dragIcon != null)
    {
        Destroy(dragIcon);
        dragIcon = null;
    }

    SyncFromSystemSlot();
    Debug.Log("OnEndDrag: equippedItem = " + (equippedItem != null ? equippedItem.name : "null"));

    bool movedToInventory = false;

    if (inventorySystem != null && equippedItem != null)
    {
        var invSlotUI = eventData.pointerEnter?.GetComponentInParent<InventorySlotUI>();
        Debug.Log("pointerEnter: " + (eventData.pointerEnter != null ? eventData.pointerEnter.name : "null"));
        Debug.Log("invSlotUI: " + invSlotUI);
        if (invSlotUI != null)
        {
            var slot = inventorySystem.GetSlot(invSlotUI.slotIndex);
            
                slot.item = equippedItem;
                slot.count = equippedCount;
                movedToInventory = true;
                Debug.Log("1111");
            
            
        }
        else
        {
            // Якщо не на слот — шукаємо перший вільний звичайний слот
            foreach (var slot in inventorySystem._inventorySlots)
            {
                if (slot.item == null)
                {
                    slot.item = equippedItem;
                    slot.count = equippedCount;
                    movedToInventory = true;
                    Debug.Log("2222");
                    break;
                }
            }
        }
    }

    // Очищаємо спецслот і зберігаємо стан тільки якщо предмет переміщено у звичайний слот
    if (movedToInventory)
    {
        SetSystemSlot(null, 0);
        equippedItem = null;
        equippedCount = 0;
        Debug.Log("adsasd");
        inventorySystem.SaveInventory();
        inventorySystem.inventoryUI.Refresh(inventorySystem._inventorySlots);
        UpdateIcon();
    }
    else
    {
        UpdateIcon();
    }
}
    public ItemData GetEquippedItem() => equippedItem;
    public void SetEquippedItem(ItemData item)
    {
        equippedItem = item;
        UpdateIcon();
    }

    public int GetEquippedCount() => equippedCount;
    public void ClearEquipped()
    {
        equippedItem = null;
        equippedCount = 0;
        UpdateIcon();
    }
}