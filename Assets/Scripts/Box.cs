using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] private GameObject _hintPrefab;
    [Header("Drop Item Settings")]
    [SerializeField] private GameObject[] _itemPrefab; // Перфаб предмета, який випадає з коробки
    [SerializeField] private int _dropChance = 50; // Шанс випаду предмета (0-100)
    private GameObject _hintInstance;
    private bool _isOpened = false;
    private bool _playerNear = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool("isOpen", false);
    }

    void Update()
    {
        if (!_isOpened && _playerNear && Input.GetKeyDown(KeyCode.E))
        {
            _isOpened = true;
            _animator.SetBool("isOpen", true);
            ShowHint(false);
            DropItem();
            Debug.Log("Box opened!");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isOpened && collision != null && collision.CompareTag("Player"))
        {
            _playerNear = true;
            ShowHint(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            _playerNear = false;
            ShowHint(false);
        }
    }

    private void ShowHint(bool show)
    {
        if (show)
        {
            if (_hintInstance == null && _hintPrefab != null)
            {
                _hintInstance = Instantiate(_hintPrefab, transform);
                _hintInstance.transform.localPosition = Vector3.up * 1f;
            }
        }
        else
        {
            if (_hintInstance != null)
            {
                Destroy(_hintInstance);
                _hintInstance = null;
            }
        }
    }
    private void DropItem()
    {
        if (_itemPrefab != null)
        {
            float dropRadius = 1.5f;
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
            Vector3 dropPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            GameObject item = null;
            if (Random.Range(0, 100) < _dropChance)
            {
                item = Instantiate(_itemPrefab[0], dropPosition, Quaternion.identity);
            }
            else
            {
                item = Instantiate(_itemPrefab[1], dropPosition, Quaternion.identity);
            }
            // Додаємо ефект розльоту
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = randomOffset.normalized * Random.Range(0.7f, 1.2f) + Vector2.up * Random.Range(0.5f, 1.0f);
                rb.AddForce(force, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-10f, 10f));
                rb.drag = 3f; // швидке гальмування
                rb.angularDrag = 3f;
            }
            Debug.Log($"Item dropped from box at {dropPosition}!");
        }
        else
        {
            Debug.LogWarning("Item prefab is not assigned in Box_Drop script.");
        }
    }
}