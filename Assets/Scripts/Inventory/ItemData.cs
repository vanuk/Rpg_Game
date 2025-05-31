using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public bool isStackable;
    public GameObject prefab;
    public EquipmentType equipmentType; // Додано тип екіпіровки
    public EquipmentType itemType; // Додано поле itemType (EquipmentType) для коректної роботи інспектора та інвентаря
    public int maxStack = 1;
    //[HideInInspector]
    public string uniqueID;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = Guid.NewGuid().ToString();
    } // Додано поле maxStack для підтримки відображення максимального стека предмета у редакторі та інвентарі
}
