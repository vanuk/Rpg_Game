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
    public List<ItemSlot> _inventorySlots = new List<ItemSlot>();
    public List<ItemSlot> equipmentSlots = new List<ItemSlot>(6);
    public InventoryUI inventoryUI;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject drad;

    private const string InventoryKey = "InventoryData";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (equipmentSlots.Count < 6)
        {
            equipmentSlots.Clear();
            for (int i = 0; i < 6; i++)
                equipmentSlots.Add(new ItemSlot(null, 0));
        }

        LoadInventory();
        BroadcastMessage("SyncFromSystemSlot", SendMessageOptions.DontRequireReceiver);
    }

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (drad != null)
                drad.SetActive(!drad.activeSelf);
        }
    }

    void OnApplicationQuit()
    {
        SaveInventory();
    }

    void OnDisable()
    {
        SaveInventory();
    }

    public void SwapSlots(int from, int to)
    {
        if (from < 0 || from >= _inventorySlots.Count || to < 0 || to >= _inventorySlots.Count || from == to)
            return;

        var temp = _inventorySlots[from];
        _inventorySlots[from] = _inventorySlots[to];
        _inventorySlots[to] = temp;
        inventoryUI.Refresh(_inventorySlots);
    }

    public void AddItem(ItemData item)
    {
        foreach (var slot in _inventorySlots)
        {
            if (slot.item == item && slot.count < 10)
            {
                slot.count++;
                inventoryUI.Refresh(_inventorySlots);
                return;
            }
        }

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
    
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count)
            return;

        _inventorySlots[slotIndex].item = null;
        _inventorySlots[slotIndex].count = 0;
        inventoryUI.Refresh(_inventorySlots);
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

    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();
        saveData.itemIDs = new string[_inventorySlots.Count];
        saveData.itemCounts = new int[_inventorySlots.Count];

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            saveData.itemIDs[i] = _inventorySlots[i].item != null ? _inventorySlots[i].item.name : "";
            saveData.itemCounts[i] = _inventorySlots[i].count;
        }

        saveData.equipmentIDs = new string[6];
        saveData.equipmentCounts = new int[6];

        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            saveData.equipmentIDs[i] = equipmentSlots[i].item != null ? equipmentSlots[i].item.name : "";
            saveData.equipmentCounts[i] = equipmentSlots[i].item != null ? equipmentSlots[i].count : 0;
        }
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(InventoryKey, json);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey(InventoryKey))
        {
            string json = PlayerPrefs.GetString(InventoryKey);
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

            if (saveData != null && saveData.itemIDs != null)
            {
                _inventorySlots.Clear();
                for (int i = 0; i < saveData.itemIDs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(saveData.itemIDs[i]))
                    {
                        ItemData item = Resources.Load<ItemData>(saveData.itemIDs[i]);
                        if (item != null)
                        {
                            _inventorySlots.Add(new ItemSlot(item, saveData.itemCounts[i]));
                        }
                        else
                        {
                            _inventorySlots.Add(new ItemSlot(null, 0));
                        }
                    }
                    else
                    {
                        _inventorySlots.Add(new ItemSlot(null, 0));
                    }
                }
            }

            if (saveData.equipmentIDs != null && saveData.equipmentCounts != null && saveData.equipmentIDs.Length == 6)
            {
                for (int i = 0; i < equipmentSlots.Count; i++)
                {
                    if (!string.IsNullOrEmpty(saveData.equipmentIDs[i]))
                    {
                        ItemData item = Resources.Load<ItemData>(saveData.equipmentIDs[i]);
                        equipmentSlots[i].item = item;
                        equipmentSlots[i].count = saveData.equipmentCounts[i];
                    }
                    else
                    {
                        equipmentSlots[i].item = null;
                        equipmentSlots[i].count = 0;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class InventorySaveData
{
    public string[] itemIDs;
    public int[] itemCounts;
    public string[] equipmentIDs;
    public int[] equipmentCounts;
}