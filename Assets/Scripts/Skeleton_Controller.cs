using UnityEngine;
using System.Collections;

public class Skeleton_Controller : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 100;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _attackDownOffset = 0.5f;
    [SerializeField] private float _attackCoolLeft=0.1f; // Offset for the attack range downwards
    [SerializeField] private float _searchRange = 5f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private int _damage = 10;
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private float _patrolPointReachedThreshold = 0.1f;
    [SerializeField] private GameObject _deadSprite;
    private int _currentPatrolIndex = 0;
    private Transform _target;
    private bool _isChasing = false;
    private bool _isAttacking = false;
    private bool _isDead = false;
    private float _timeSinceLastAttack = 0f;
    [SerializeField] private float _attackCooldown = 2f;
    private Transform _playerTransform;

    void Start()
    {
        _target = _patrolPoints[_currentPatrolIndex];
        _animator.SetBool("move", true);
    }

    void Update()
    {
        if (_isDead) return;

        if (_isChasing)
        {
            ChaseAndAttack();
        }
        else
        {
            Patrol();
        }

        _timeSinceLastAttack += Time.deltaTime;
    }

    void Patrol()
    {
        if (_target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, _target.position);

        if (distanceToTarget <= _patrolPointReachedThreshold)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _target = _patrolPoints[_currentPatrolIndex];
            Flip();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            _animator.SetBool("move", true);

            // Flip towards the patrol point
            if (_target.position.x > transform.position.x && transform.localScale.x < 0 ||
                _target.position.x < transform.position.x && transform.localScale.x > 0)
            {
                Flip();
            }
        }

        // Check for player in search range
        Collider2D player = Physics2D.OverlapCircle(transform.position, _searchRange, _playerLayer);
        if (player != null)
        {
            _isChasing = true;
            _animator.SetBool("move", false);
            _playerTransform = player.transform;
        }
    }

    void ChaseAndAttack()
{
    if (_playerTransform == null)
    {
        _isChasing = false;
        _animator.SetBool("attack", false);
        _animator.SetBool("move", true);
        _target = _patrolPoints[_currentPatrolIndex];
        if (_target.position.x > transform.position.x && transform.localScale.x < 0 ||
            _target.position.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
        return;
    }

    // Перевірка на атаку через OverlapCircle
    Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y);
    Collider2D playerInRange = Physics2D.OverlapCircle(attackPosition, _attackRange, _playerLayer);

    float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

    // Flip towards the player
    if (_playerTransform.position.x > transform.position.x && transform.localScale.x < 0 ||
        _playerTransform.position.x < transform.position.x && transform.localScale.x > 0)
    {
        Flip();
    }

    if (playerInRange != null)
    {
        _animator.SetBool("move", false);
        if (_timeSinceLastAttack >= _attackCooldown)
        {
            Attack();
            _timeSinceLastAttack = 0;
        }
        else
        {
            _animator.SetBool("attack", false);
        }
    }
    else if (distanceToPlayer > _searchRange)
    {
        _isChasing = false;
        _animator.SetBool("attack", false);
        _animator.SetBool("move", true);
        _target = _patrolPoints[_currentPatrolIndex];
        if (_target.position.x > transform.position.x && transform.localScale.x < 0 ||
            _target.position.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }
    else
    {
        // Рухаємось до гравця, поки не потрапимо в зону атаки
        Vector2 direction = (_playerTransform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, _speed * Time.deltaTime);
        _animator.SetBool("move", true);
        _animator.SetBool("attack", false);
    }
}
    void Attack()
    {
        _animator.SetBool("move", false);
        _animator.SetBool("attack", true);
        
        Vector2 attackPosition = new Vector2(transform.position.x  , transform.position.y );
 
        Collider2D player = Physics2D.OverlapCircle(attackPosition, _attackRange, _playerLayer);

        if (player != null)
        {
            Move_Soldier playerScript = player.GetComponent<Move_Soldier>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(_damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _health -= damage;
        _animator.SetTrigger("hit");

        if (_health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        _isDead = true;
        _animator.SetBool("dead", true);
        _animator.SetBool("move", false);
        _animator.SetBool("attack", false);
        _animator.SetBool("hit", false);

        _animator.SetTrigger("dead1");
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // Wait for the death animation to finish
        yield return new WaitForSeconds(1.8f); // Adjust the duration to match your animation length

        // Instantiate the dead sprite
        if(_deadSprite != null)
        {
             Instantiate(_deadSprite, transform.position, Quaternion.identity);
        }
       
        // Destroy the skeleton object
        Destroy(gameObject);
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Draw the original attack range
        //Gizmos.DrawWireSphere(transform.position, _attackRange);

        // Draw the shifted attack range
        Gizmos.color = Color.green;
        Vector2 attackPosition = new Vector2(transform.position.x , transform.position.y  );
        Gizmos.DrawWireSphere(attackPosition, _attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackCooldown);
    }
}