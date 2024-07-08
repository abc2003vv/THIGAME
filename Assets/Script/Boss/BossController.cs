using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 10f;
    public float moveSpeed = 2f;
    public int maxHealth = 100;
    public int health = 100;

    public GameObject attackArea; // GameObject con với BoxCollider2D đặt là isTrigger
    public float attackCooldown = 1f; // Thời gian hồi tấn công
    private float lastAttackTime;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isPlayerInAttackArea = false; // Biến cờ để kiểm tra xem nhân vật có ở trong vùng tấn công không

    private enum State { Idle, Run, Attack, Hit, Dead }
    private State currentState = State.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        lastAttackTime = Time.time - attackCooldown; // Đảm bảo boss có thể tấn công ngay lập tức khi game bắt đầu
    }

    void Update()
    {
        // Kiểm tra nếu player không bị hủy hoặc null
        if (player == null)
        {
            return;
        }

        if (health <= 0)
        {
            ChangeState(State.Dead);
            Destroy(gameObject, 0.5f);
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (isPlayerInAttackArea)
            {
                ChangeState(State.Attack);
            }
            else if (distanceToPlayer <= chaseRange)
            {
                ChangeState(State.Run);
            }
            else
            {
                ChangeState(State.Idle);
            }
        }

        HandleState();
        FlipTowardsPlayer();
    }

    void ChangeState(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            animator.SetTrigger(newState.ToString());
        }
    }

    void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                rb.velocity = Vector2.zero;
                break;
            case State.Run:
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
                break;
            case State.Attack:
                rb.velocity = Vector2.zero;
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    lastAttackTime = Time.time;
                    AttackPlayer();
                }
                break;
            case State.Hit:
                rb.velocity = Vector2.zero;
                // Handle hit reaction
                break;
            case State.Dead:
                rb.velocity = Vector2.zero;
                // Handle death
                break;
        }
    }

    void AttackPlayer()
    {
        // Logic to deal damage to the player
        if (player != null)
        {
            player.GetComponent<CharacterController>().TakeDamage(10);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == player)
        {
            isPlayerInAttackArea = true; // Đặt là true khi nhân vật vào vùng tấn công
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == player)
        {
            isPlayerInAttackArea = false; // Đặt là false khi nhân vật rời khỏi vùng tấn công
        }
    }

    void FlipTowardsPlayer()
    {
        if ((player.position.x < transform.position.x && !isFacingRight) || (player.position.x > transform.position.x && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState != State.Dead)
        {
            health -= damage;
            if (health > 0)
            {
                ChangeState(State.Hit);
            }
            else
            {
                ChangeState(State.Dead);
            }
        }
    }
}
