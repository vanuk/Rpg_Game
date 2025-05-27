using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public ItemData itemData;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Inventory_System inv = FindObjectOfType<Inventory_System>();
            if (inv != null && itemData != null)
            {
                inv.AddItem(itemData);
                Destroy(gameObject);
            }
        }
    }
}