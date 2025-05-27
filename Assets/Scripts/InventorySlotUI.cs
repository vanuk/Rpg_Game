using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public int slotIndex;
    public Inventory_System inventorySystem;

    private GameObject dragIcon;
    private Canvas canvas;

    // Для підсвічування вибраного слота та виводу опису
    public static InventorySlotUI lastSelectedSlot;
    public Text descriptionText; // Признач у інспекторі або з коду
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
{
    // Повертаємо попередній слот до нормального кольору
    if (lastSelectedSlot != null && lastSelectedSlot != this)
    {
        var prevIcon = lastSelectedSlot.transform.Find("Icon")?.GetComponent<Image>();
        if (prevIcon != null)
        {
            var prevColor = prevIcon.color;
            prevColor.a = 1f;
            prevIcon.color = prevColor;
        }
    }

    var slot = inventorySystem.GetSlot(slotIndex);
    if (slot != null && slot.item != null)
    {
        Transform iconTransform = transform.Find("Icon");
        if (iconTransform != null)
        {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                var color = iconImage.color;
                color.a = 0.5f; // Мутний
                iconImage.color = color;
            }
        }
        Debug.Log(slot.item.description);
        if (descriptionText != null)
            descriptionText.text = slot.item.description;
    }
    else
    {
        if (descriptionText != null)
            descriptionText.text = "";
    }

    lastSelectedSlot = this;
}

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventorySystem == null) return;
        var slot = inventorySystem.GetSlot(slotIndex);
        if (slot == null || slot.item == null) return; // Заборона drag для порожнього слота

        Transform iconTransform = transform.Find("Icon");
        if (iconTransform == null) return;
        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null || iconImage.sprite == null) return;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        var image = dragIcon.AddComponent<Image>();
        image.sprite = iconImage.sprite;
        image.raycastTarget = false;
        image.SetNativeSize();

        RectTransform rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = iconImage.rectTransform.sizeDelta;
        rt.position = eventData.position;

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
        else
        {
            // Якщо dragIcon не було — drag не починався, нічого не робимо!
            return;
        }
        // Перевіряємо, чи дроп був не на слот
        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InventorySlotUI>() == null)
        {
            // Знайти найближчий вільний слот
            int nearestFree = -1;
            float minDist = float.MaxValue;
            Vector2 dragPos = eventData.position;

            for (int i = 0; i < inventorySystem.GetMaxSlots(); i++)
            {
                var slot = inventorySystem.GetSlot(i);
                if (slot != null && slot.item == null)
                {
                    // Знайти UI-об'єкт слота
                    var slotUIs = FindObjectsOfType<InventorySlotUI>();
                    foreach (var slotUI in slotUIs)
                    {
                        if (slotUI.slotIndex == i)
                        {
                            float dist = Vector2.Distance(slotUI.transform.position, dragPos);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                nearestFree = i;
                            }
                        }
                    }
                }
            }

            // Якщо знайдено вільний слот — перемістити предмет туди
            if (nearestFree != -1 && nearestFree != slotIndex)
            {
                inventorySystem.SwapSlots(slotIndex, nearestFree);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlot != null && draggedSlot != this)
        {
            var slotData = inventorySystem.GetSlot(draggedSlot.slotIndex);
            if (slotData == null || slotData.item == null) return;

            inventorySystem.SwapSlots(draggedSlot.slotIndex, slotIndex);
        }
    }
}