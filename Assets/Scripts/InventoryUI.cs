using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotsParent;
    public Sprite emptySlotSprite; // Додай у інспекторі спрайт для порожнього слота
    private List<GameObject> slotInstances = new List<GameObject>();

    void Start()
    {
        Refresh(new List<ItemData>());
        slotPrefab.GetComponent<Image>();
}
    public void Refresh(List<ItemData> items)
    {
        // Видалити старі слоти
        foreach (var slot in slotInstances)
            Destroy(slot);
        slotInstances.Clear();

        // Дізнаємося максимальну кількість слотів
        int maxSlots = 10;
        Inventory_System inv = FindObjectOfType<Inventory_System>();
        if (inv != null)
            maxSlots = inv.GetMaxSlots();

        // Створити слоти
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slotInstances.Add(slot);

            // Знаходимо Image "Icon" у слоті
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            if (i < items.Count && items[i] != null && items[i].icon != null)
            {
                icon.sprite = items[i].icon;
                icon.color = Color.white;
            }
            else
            {
                icon.sprite = emptySlotSprite;
                icon.color = Color.white;
            }
        }
    }
}