using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Tham chiếu đến Slider UI
    private BossController boss; // Tham chiếu đến BossController để lấy thông tin sức khỏe

    void Start()
    {
        // Tìm Boss trong cảnh và thiết lập giá trị thanh máu
        boss = FindObjectOfType<BossController>();
        if (boss != null)
        {
            healthSlider.maxValue = boss.maxHealth; // Thiết lập giá trị tối đa của Slider
            healthSlider.value = boss.health; // Thiết lập giá trị hiện tại của Slider
        }
    }

    void Update()
    {
        if (boss != null)
        {
            // Cập nhật giá trị thanh máu theo sức khỏe của Boss
            healthSlider.value = boss.health;
        }
    }
}
