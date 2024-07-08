using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CharacterController : MonoBehaviour
{
    // Di chuyển
    public float speed = 10f; // Tốc độ di chuyển
    Rigidbody2D rb; // RigidBody2D của nhân vật

    // Hướng di chuyển
    bool isFacingR = true; // Kiểm tra xem nhân vật đang đối diện bên phải hay không

    // Nhảy
    public float jumpForce = 18f; // Lực nhảy
    private int jumpCount = 0; // Đếm số lần nhảy
    public int maxJumpCount = 2; // Số lần nhảy tối đa (bao gồm nhảy đôi)
    private bool canDoubleJump; // Cờ kiểm tra khả năng nhảy đôi

    // Kiểm tra mặt đất
    [SerializeField] Transform groundCheck; // Vị trí kiểm tra mặt đất
    [SerializeField] LayerMask whatIsGround; // Mặt đất được coi là mặt đất
    [SerializeField] float groundCheckRadius; // Bán kính kiểm tra mặt đất
    bool isGrounded; // Kiểm tra xem nhân vật có đứng trên mặt đất không

    // Hoạt ảnh
    Animator anim; // Animator của nhân vật
    bool isRunning; // Kiểm tra xem nhân vật có đang chạy không

    // Tấn công
    private float attackCooldown = 0.4f; // Thời gian hồi chiêu của tấn công
    private float attack2Cooldown = 0.3f; // Thời gian hồi chiêu của tấn công thứ hai
    private float attackTimer; // Bộ đếm thời gian hồi chiêu của tấn công
    private float attack2Timer; // Bộ đếm thời gian hồi chiêu của tấn công thứ hai

    // Máu
    public int maxHealth = 100; // Máu tối đa
    public int currentHealth; // Máu hiện tại
    public HealthController healthController; // Điều khiển sức khỏe của nhân vật

    // Chết
    bool isDead = false; // Kiểm tra xem nhân vật đã chết chưa

    // Đồng xu
    public int coinCount = 0; // Số lượng đồng xu hiện tại
    public TMP_Text coinText; // Text hiển thị số lượng đồng xu

    // GameOverUI
    public GameManager gameManager; // Điều khiển giao diện Game Over

    // Hạt bụi
    public ParticleSystem dust; // Hạt bụi khi nhân vật di chuyển hoặc nhảy

    // Đạn
    public GameObject depLao; // Prefab của đạn
    [SerializeField] GameObject depLaoPoint; // Vị trí bắn đạn

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy RigidBody2D của nhân vật
        anim = GetComponent<Animator>(); // Lấy Animator của nhân vật

        // Khởi tạo máu
        currentHealth = maxHealth;
        healthController.maxHealth = maxHealth;

        depLao = Resources.Load<GameObject>("Deplao"); // Tải prefab đạn từ tài nguyên

        // Khởi tạo giá trị của đồng xu khi bắt đầu
        UpdateCoinUI();
    }

    void Update()
    {
        if (isDead) return; // Nếu nhân vật đã chết, không thực hiện các hành động khác

        // Di chuyển
        float move = Input.GetAxisRaw("Horizontal"); // Lấy giá trị di chuyển ngang từ Input
        rb.velocity = new Vector2(speed * move, rb.velocity.y); // Cập nhật vận tốc

        // Kiểm tra mặt đất
        isGrounded = IsGrounded(); // Kiểm tra xem nhân vật có đứng trên mặt đất không

        // Hoạt ảnh chạy
        isRunning = Mathf.Abs(move) > 0; // Kiểm tra xem có di chuyển hay không
        anim.SetBool("isRunning", isRunning); // Cập nhật trạng thái chạy

        // Hướng di chuyển
        if (move > 0 && !isFacingR)
        {
            Flip(); // Đảo hướng khi di chuyển sang phải
        }
        else if (move < 0 && isFacingR)
        {
            Flip(); // Đảo hướng khi di chuyển sang trái
        }

        // Nhảy
        if (isGrounded)
        {
            jumpCount = 0; // Đặt lại số lần nhảy khi đứng trên mặt đất
            canDoubleJump = true; // Cho phép nhảy đôi khi đứng trên mặt đất
            anim.SetBool("isJumping", false); // Đặt lại trạng thái nhảy
            anim.SetBool("isFalling", false); // Đặt lại trạng thái rơi
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isGrounded || (canDoubleJump && jumpCount < maxJumpCount))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Thực hiện nhảy
                jumpCount++; // Tăng số lần nhảy
                anim.SetBool("isJumping", true); // Cập nhật trạng thái nhảy
                anim.SetBool("isFalling", false); // Đảm bảo trạng thái rơi là false khi nhảy
                createDust(); // Tạo hạt bụi khi nhảy

                if (!isGrounded)
                {
                    canDoubleJump = false; // Ngăn không cho nhảy đôi thêm nếu đã nhảy
                }
            }
        }

        // Tấn công
        if (Input.GetKeyDown(KeyCode.Z) && attackTimer <= 0)
        {
            Attack(); // Thực hiện tấn công
        }

        // Tấn công 2
        if (Input.GetKeyDown(KeyCode.C) && attack2Timer <= 0)
        {
            Attack2(); // Thực hiện tấn công thứ hai
        }

        // Cập nhật trạng thái rơi
        UpdateFallingState();

        // Cập nhật bộ đếm thời gian hồi chiêu
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; // Giảm thời gian hồi chiêu của tấn công
        }

        if (attack2Timer > 0)
        {
            attack2Timer -= Time.deltaTime; // Giảm thời gian hồi chiêu của tấn công thứ hai
        }
    }

    void Flip()
    {
        isFacingR = !isFacingR; // Đảo hướng nhân vật
        Vector3 theScale = transform.localScale; // Lấy kích thước hiện tại của nhân vật
        theScale.x *= -1f; // Đảo hướng theo trục x
        transform.localScale = theScale; // Cập nhật kích thước mới
        createDust(); // Tạo hạt bụi khi đảo hướng
    }

    void Attack()
    {
        anim.SetBool("isAttacking", true); // Cập nhật trạng thái tấn công
        attackTimer = attackCooldown; // Đặt thời gian hồi chiêu cho tấn công
        Invoke("ResetAttack", attackCooldown); // Lên lịch reset trạng thái tấn công

        // Kiểm tra va chạm với thùng (box)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Box"))
            {
                BoxController box = hitCollider.GetComponent<BoxController>();
                if (box != null)
                {
                    box.DestroyBox(); // Gọi phương thức DestroyBox của thùng
                }
            }
        }
    }

    void ResetAttack()
    {
        anim.SetBool("isAttacking", false); // Đặt lại trạng thái tấn công
    }

    void Attack2()
    {
        anim.SetBool("isAttacking2", true); // Cập nhật trạng thái tấn công thứ hai
        attack2Timer = attack2Cooldown; // Đặt thời gian hồi chiêu cho tấn công thứ hai
        Invoke("ResetAttack2", attack2Cooldown); // Lên lịch reset trạng thái tấn công thứ hai

        GameObject aDepLao = Instantiate(depLao, depLaoPoint.transform.position, Quaternion.identity); // Tạo đạn

        // Đảo hướng của đạn tùy thuộc vào hướng của nhân vật
        if (!isFacingR)
        {
            aDepLao.GetComponent<Rigidbody2D>().velocity = -transform.right * 10f; // Đạn đi theo hướng trái
        }
        else
        {
            aDepLao.GetComponent<Rigidbody2D>().velocity = transform.right * 10f; // Đạn đi theo hướng phải
        }
        Destroy(aDepLao, 2f); // Hủy đạn sau 2 giây
    }

    void ResetAttack2()
    {
        anim.SetBool("isAttacking2", false); // Đặt lại trạng thái tấn công thứ hai
    }

    void UpdateFallingState()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            anim.SetBool("isFalling", true); // Cập nhật trạng thái rơi nếu không còn đứng trên mặt đất và đang rơi xuống
        }
        else if (isGrounded)
        {
            anim.SetBool("isFalling", false); // Đặt lại trạng thái rơi khi đứng trên mặt đất
        }
    }

    public void TriggerHitEffect()
    {
        // Kích hoạt hiệu ứng bị đánh trong Animator
        anim.SetTrigger("isHitting");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Nếu nhân vật đã chết, không thực hiện việc nhận sát thương

        // Giảm máu của nhân vật
        currentHealth -= damage;
        healthController.TakeDamage(damage); // Cập nhật sức khỏe

        if (currentHealth <= 0 && !isDead)
        {
            gameManager.gameOver(); // Gọi GameManager để xử lý kết thúc trò chơi
            Death(); // Xử lý cái chết của nhân vật
        }
    }

    // Tăng máu
    void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Tăng máu và không vượt quá maxHealth
        healthController.Heal(amount); // Cập nhật sức khỏe

        // Có thể thêm hiệu ứng hoặc âm thanh khi hồi máu
    }

    // Xử lý cái chết
    public void Death()
    {
        isDead = true; // Đánh dấu nhân vật đã chết
        anim.SetTrigger("isDeathing"); // Kích hoạt hiệu ứng chết trong Animator
        rb.velocity = Vector2.zero; // Dừng chuyển động
        rb.isKinematic = true; // Vô hiệu hóa tương tác vật lý
        StartCoroutine(HandleDeath()); // Bắt đầu coroutine để xử lý cái chết
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.6f); // Chờ 0.6 giây (thay đổi tùy thuộc vào độ dài hoạt ảnh chết)
        Destroy(gameObject); // Hủy đối tượng nhân vật
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround); // Kiểm tra xem nhân vật có đứng trên mặt đất không
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Màu sắc của Gizmo
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // Vẽ hình cầu để kiểm tra mặt đất
    }

    // Xử lý đồng xu
    public void collecCoin(int amount)
    {
        coinCount += amount; // Tăng số lượng đồng xu
        UpdateCoinUI(); // Cập nhật giao diện đồng xu
    }

    public void UpdateCoinUI()
    {
        coinText.text = coinCount.ToString(); // Cập nhật text hiển thị số lượng đồng xu
    }

    // Xử lý va chạm với kẻ thù và ngược lại
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Nếu nhân vật đã chết, không thực hiện các hành động khác

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
                    anim.SetTrigger("isHitting"); // Kích hoạt hiệu ứng bị đánh
                }
            }
        }

        if (collision.gameObject.CompareTag("Box"))
        {
            foreach (ContactPoint2D contactBox in collision.contacts)
            {
                if (contactBox.point.y < transform.position.y - 0.9f)
                {
                    BoxController box = collision.gameObject.GetComponent<BoxController>();
                    if (box != null)
                    {
                        box.DestroyBox(); // Gọi phương thức DestroyBox của thùng
                    }
                }
            }
        }
    }

    // Tạo hạt bụi
    void createDust()
    {
        dust.Play(); // Phát hệ thống hạt bụi
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return; // Nếu nhân vật đã chết, không thực hiện các hành động khác

        if (collision.CompareTag("BloodTonic"))
        {
            RedHeathUp bloodTonic = collision.GetComponent<RedHeathUp>();
            if (bloodTonic != null)
            {
                Heal(bloodTonic.healthAmount); // Tăng máu cho nhân vật
                bloodTonic.conSume(); // Hủy đối tượng BloodTonic
            }
        }
    }
}
