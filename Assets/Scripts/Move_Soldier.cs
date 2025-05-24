using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Move_Soldier : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Animator _animator;
    [Header("Health Settings")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Slider _healthBar;
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private LayerMask _enemyLayer;
    private Rigidbody2D _rb;
    [Header("Damage Settings")]
    [SerializeField] private int _damage = 10;
    [Header("UI Settings")]
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private Text _coinsText;
    private int _coins = 0;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _animator.SetBool("move", false);

        _health= _maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _health;
        
        _restartButton.SetActive(false);
    }

    void Update()
    {
        AnimationAttack();
        //TemporaryFunction();
    }
    void FixedUpdate()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            _animator.SetBool("move", true);
        }
        else
        {
            _animator.SetBool("move", false);
        }

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;
       
        Move(moveDirection);
       
        Flip(horizontalInput);
    }

    public void Move(Vector3 direction)
    {
        transform.position += direction * _speed * Time.deltaTime;
    }
    public void Flip(float horizontalInput)
    {
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public void TakeDamage(int damage)
    {   
        _health -= damage;
        _animator.SetTrigger("hit");
        _healthBar.value = _health;

        if (_health <= 0 )
        {
            Die();
        }
    }
   

    public void Die()
    {
        _animator.SetBool("dead", true);
        StartCoroutine(PauseAndShowRestart());
    }
    IEnumerator PauseAndShowRestart()
    {
        yield return new WaitForSeconds(0.7f);
        Time.timeScale = 0; 
        _restartButton.SetActive(true);
    }
    public void RestartGame()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    // public void TemporaryFunction()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         _animator.SetBool("hit", true);
    //         TakeDamage(10); 
    //     }
    //     else if (Input.GetKeyUp(KeyCode.T))
    //     {
    //         _animator.SetBool("hit", false);
    //     }
    // }

    public void AnimationAttack()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            int randomAttack = Random.Range(0, 2);
            if(randomAttack == 0)
            {
                Attack();
                _animator.SetBool("attack", true);
            }
            else if(randomAttack == 1)
            {
                Attack();
                _animator.SetBool("attack1", true);
            }
        }
        else if(Input.GetKeyUp(KeyCode.F))
        {
            _animator.SetBool("attack", false);
            _animator.SetBool("attack1", false);
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
            Skeleton_Controller enemyComponent = enemy.GetComponent<Skeleton_Controller>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(_damage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            _coins++;
            _coinsText.text = "Coins: " + _coins;
            Destroy(collision.gameObject);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}