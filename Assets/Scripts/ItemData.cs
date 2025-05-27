using UnityEngine;

public enum ItemType { Default, Weapon, Armor, Consumable }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public GameObject prefab;
    [TextArea] public string description;
    public int maxStack = 10;
}