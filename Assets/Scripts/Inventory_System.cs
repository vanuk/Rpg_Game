using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int count;

    public ItemSlot(ItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

public class Inventory_System : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 10;
    [SerializeField] private List<ItemSlot> _inventorySlots = new List<ItemSlot>();
    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject drad;

    void Start()
    {
        // Заповнюємо порожні слоти
        while (_inventorySlots.Count < _maxSlots)
            _inventorySlots.Add(new ItemSlot(null, 0));

        if (inventoryUI != null)
            inventoryUI.Refresh(_inventorySlots);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (drad != null)
            drad.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // або інша клавіша
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (drad != null)
                drad.SetActive(!drad.activeSelf);
        }
    }

    public void SwapSlots(int from, int to)
    {
        if (from < 0 || from >= _inventorySlots.Count || to < 0 || to >= _inventorySlots.Count || from == to)
            return;

        // Міняємо місцями слоти
        var temp = _inventorySlots[from];
        _inventorySlots[from] = _inventorySlots[to];
        _inventorySlots[to] = temp;
        inventoryUI.Refresh(_inventorySlots);
    }

    public void AddItem(ItemData item)
    {
        // Спроба знайти вже існуючий стак
        foreach (var slot in _inventorySlots)
        {
            if (slot.item == item && slot.count < 10)
            {
                slot.count++;
                inventoryUI.Refresh(_inventorySlots);
                return;
            }
        }

        // Додаємо у перший порожній слот
        foreach (var slot in _inventorySlots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.count = 1;
                inventoryUI.Refresh(_inventorySlots);
                return;
            }
        }
    }

    public int GetMaxSlots()
    {
        return _maxSlots;
    }

    public ItemSlot GetSlot(int index)
    {
        if (index < 0 || index >= _inventorySlots.Count) return null;
        return _inventorySlots[index];
    }
}