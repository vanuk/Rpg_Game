using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_System : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 10;
    [SerializeField] private List<ItemData> _inventoryItems = new List<ItemData>();
    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryPanel; // Панель інвентаря (наприклад, panel7)

    public void AddItem(ItemData item)
    {
        if (_inventoryItems.Count < _maxSlots)
        {
            _inventoryItems.Add(item);
            inventoryUI.Refresh(_inventoryItems);
        }
    }

    public void RemoveItem(ItemData item)
    {
        if (_inventoryItems.Contains(item))
        {
            _inventoryItems.Remove(item);
            inventoryUI.Refresh(_inventoryItems);
        }
    }
    public int GetMaxSlots()
{
    return _maxSlots;
}
    void Update()
    {
        // Відкрити/закрити інвентар по кнопці I
        if (Input.GetKeyDown(KeyCode.I) && inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

}
