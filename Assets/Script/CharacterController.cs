using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class CharacterController : MonoBehaviour
{
    // Movement
    public float speed = 10f;
    Rigidbody2D rb;
    // Facing direction
    bool isFacingR = true;
    // Jumping
    public float jumpForce = 20f;
    private int jumpCount = 0;
    public int maxJumpCount = 2;
    // Ground check
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    bool isGrounded;
    // Animation
    Animator anim;
    bool isRunning;
    //Attack
    private float attackCooldown = 0.4f;
    private float attack2Cooldown = 0.3f;
    private float attackTimer;
    private float attack2Timer;

    // Health
    public int maxHealth = 100;
    public int currentHealth;
    public HealthController healthController;
    //public HeathBar heathBar;
    // Death
    bool isDead = false;
    //Coin
    public int coinCount = 0;
    public TMP_Text coinText;
    //GameOverUI
    public GameManager gameManager;
    //DustPS
    public ParticleSystem dust;
    //Dep Lao
    public GameObject depLao;
    [SerializeField] GameObject depLaoPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Health
        currentHealth = maxHealth;
        healthController.maxHealth = maxHealth;

        depLao = Resources.Load<GameObject>("Deplao");
        // Khởi tạo giá trị của goldTextTMP khi bắt đầu
        UpdateCoinUI();
    }

    void Update()
    {
        if (isDead) return;

        // Movement
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(speed * move, rb.velocity.y);

        // Ground check
        //isGrounded = IsGrounded();

        // Jumping
        if (IsGrounded())
        {
            jumpCount = 0; // Reset jump count when touching ground
            anim.SetBool("isJumping", false); // Reset jumping state
            anim.SetBool("isFalling", false); // Reset falling state
        }

        if (Input.GetKeyDown(KeyCode.X) && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false); // Ensure falling is false during the jump
            createDust();
        }

        // Running animation
        isRunning = Mathf.Abs(move) > 0;
        anim.SetBool("isRunning", isRunning);


        // Facing direction
        if (move > 0 && !isFacingR)
        {
            Flip();
        }
        else if (move < 0 && isFacingR)
        {
            Flip();
        }

        // Attacking
        if (Input.GetKeyDown(KeyCode.Z) && attackTimer <= 0)
        {
            Attack();
        }

        // Attacking 2
        if (Input.GetKeyDown(KeyCode.C) && attack2Timer <= 0)
        {
            Attack2();
}

        // Update falling state
        UpdateFallingState();

        // Update attack timers
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (attack2Timer > 0)
        {
            attack2Timer -= Time.deltaTime;
        }
    }

    void Flip()
    {
        isFacingR = !isFacingR;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1f;
        transform.localScale = theScale;
        createDust();
    }

    void Attack()
    {
        anim.SetBool("isAttacking", true);
        attackTimer = attackCooldown; // Set the cooldown for the attack
        Invoke("ResetAttack", attackCooldown); // Schedule attack reset

        // Kiểm tra va chạm với thùng (box)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Box"))
            {
                BoxController box = hitCollider.GetComponent<BoxController>();
                if (box != null)
                {
                    box.DestroyBox();
                }
            }
        }
    }

    void ResetAttack()
    {
        anim.SetBool("isAttacking", false);
    }

    void Attack2()
    {
        anim.SetBool("isAttacking2", true);
        attack2Timer = attack2Cooldown; // Set the cooldown for the second attack
        Invoke("ResetAttack2", attack2Cooldown); // Schedule attack reset

        GameObject aDepLao = Instantiate(depLao, depLaoPoint.transform.position, Quaternion.identity);

        // Đảo hướng của đạn (depLao) nếu nhân vật đang quay mặt trái
        if (!isFacingR)
        {
            aDepLao.GetComponent<Rigidbody2D>().velocity = -transform.right * 10f; // Đạn đi theo hướng trái
        }
        else
        {
            aDepLao.GetComponent<Rigidbody2D>().velocity = transform.right * 10f; // Đạn đi theo hướng phải
        }
        Destroy(aDepLao, 2f);
    }

    void ResetAttack2()
    {
        anim.SetBool("isAttacking2", false);
    }

    void UpdateFallingState()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            anim.SetBool("isFalling", true);
        }
        else if (isGrounded)
        {
            anim.SetBool("isFalling", false);
        }
    }

    public void TriggerHitEffect()
    {
        // Trigger Hurt in Animator
        anim.SetTrigger("isHitting");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // Reduce character's health
        currentHealth -= damage;
        //heathBar.setHealth(currentHealth);
        healthController.TakeDamage(damage);

        //Instantiate(popUpDamagePrefabs, transform.position, Quaternion.identity);

        if (currentHealth <= 0 && !isDead)
        {
            gameManager.gameOver(); //UI GameOver
            Death();
        }
    }

    //Tang mau
void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Tăng máu và không vượt quá maxHealth
        //heathBar.setHealth(currentHealth);
        healthController.Heal(amount);

        // Optionally, add any heal effect or sound here
    }

    // Death
    public void Death()
    {
        isDead = true;
        anim.SetTrigger("isDeathing");
        rb.velocity = Vector2.zero; // Stop movement
        rb.isKinematic = true; // Disable physics interactions
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.6f); // Adjust the delay to match your death animation duration
        Destroy(gameObject);
    }

    bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.down, castDistance, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

    //xu ly coin
    public void collecCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        coinText.text = coinCount.ToString();
    }


    //Nhân vật va chạm với Enemy và ngược lại 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y < transform.position.y - 0.9f) // Kiểm tra nếu chân nhân vật chạm vào enemy
                {
                    EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(); // Gọi phương thức TakeDamage của enemy
                    }
                }
                else // Kiểm tra nếu nhân vật va chạm trên lưng enemy
                {
                    TakeDamage(5); // Trừ máu khi va chạm với enemy
                    anim.SetTrigger("isHitting");
                }
            }
        }

        if (collision.gameObject.CompareTag("Box"))
        {
            foreach (ContactPoint2D contactBox in collision.contacts)
            {
                if (contactBox.point.y < transform.position.y - 1f)
                {
                    BoxController box = collision.gameObject.GetComponent<BoxController>();
                    if (box != null)
                    {
                        box.DestroyBox();
                        // Kiểm tra nếu nhân vật đang tấn công
                        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
                        {
                            box.DestroyBox(); // Gọi phương thức DestroyBox của thùng
                        }
}
                }
            }

        }

        /*
        if (collision.gameObject.CompareTag("Boom")) // Kiểm tra va chạm với Boom
        {
            TrapBoomController trapBoom = collision.gameObject.GetComponent<TrapBoomController>();
            if (trapBoom != null) // Kiểm tra xem tham chiếu đã được thiết lập chưa
            {
                trapBoom.Boom(); // Gọi phương thức Boom từ script TrapBoomController
            }
        }
        */
    }

    //Dust
    void createDust()
    {
        dust.Play();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("BloodTonic"))
        {
            RedHeathUp bloodTonic = collision.GetComponent<RedHeathUp>();
            if (bloodTonic != null)
            {
                Heal(bloodTonic.healthAmount); // Tăng máu cho nhân vật
                bloodTonic.conSume(); // Hủy 
            }
        }
    }
}