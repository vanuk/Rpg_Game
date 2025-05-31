using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotsParent;
    public Transform dragIconParent;
    public Sprite emptySlotSprite;
    public Text descriptionText; // Признач у Canvas через інспектор
    private List<GameObject> slotInstances = new List<GameObject>();

    public void Refresh(List<ItemSlot> slots)
    {

        Canvas canvas = slotsParent.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            foreach (Transform child in canvas.transform)
            {
                if (child.name == "DragIcon")
                    Destroy(child.gameObject);
            }
        }
        foreach (var slot in slotInstances)
            Destroy(slot);
        slotInstances.Clear();

        int maxSlots = 10;
        Inventory_System inv = FindObjectOfType<Inventory_System>();
        if (inv != null)
            maxSlots = inv.GetMaxSlots();

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent, dragIconParent);
            slotInstances.Add(slotGO);

            // Додаємо InventorySlotUI і передаємо індекс, Inventory_System і descriptionText
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            if (slotUI == null)
                slotUI = slotGO.AddComponent<InventorySlotUI>();
            slotUI.slotIndex = i;
            slotUI.inventorySystem = inv;
            slotUI.descriptionText = descriptionText;

            Image icon = slotGO.transform.Find("Icon").GetComponent<Image>();
            Text countText = slotGO.GetComponentInChildren<Text>();

            if (i < slots.Count && slots[i] != null && slots[i].item != null)
            {
                icon.sprite = slots[i].item.icon;
                icon.color = Color.white;
                if (countText != null)
                    countText.text = slots[i].count > 1 ? slots[i].count.ToString() : "";
            }
            else
            {
                icon.sprite = emptySlotSprite;
                icon.color = Color.white;
                if (countText != null)
                    countText.text = "";
            }
        }
    }
}