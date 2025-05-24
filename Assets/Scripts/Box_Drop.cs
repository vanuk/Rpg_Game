using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Drop : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab; // Перфаб предмета, який випадає з коробки
    [SerializeField] private int _dropChance = 50; // Шанс випаду предмета (0-100)
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Перевірка шансів на випадання предмета
            if (Random.Range(0, 100) < _dropChance)
            {
                DropItem();
            }
        }
    }
    private void DropItem()
    {
        if (_itemPrefab != null)
        {
            // Створення предмета в позиції коробки
            Instantiate(_itemPrefab, transform.position, Quaternion.identity);
            Debug.Log("Item dropped from box!");
        }
        else
        {
            Debug.LogWarning("Item prefab is not assigned in Box_Drop script.");
        }
    }
}
