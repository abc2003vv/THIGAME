using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isNearChest = false;
    private bool isChestOpened = false;

    public string question;
    public string[] answers = new string[4]; // Đáp án có 4 lựa chọn
    public int correctAnswerIndex; // Chỉ số của đáp án đúng

    public Animator chestAnimator; // Thêm Animator cho hòm rương
    public GameObject coinPrefab; // Prefab của đồng tiền
    public int numberOfCoins = 10; // Số lượng đồng tiền sẽ rơi ra

    void Update()
    {
        if (isNearChest && Input.GetKeyDown(KeyCode.V) && !isChestOpened)
        {
            if (PuzzleUI.Instance != null)
            {
                PuzzleUI.Instance.ShowPuzzle(this, question, answers, correctAnswerIndex);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            isNearChest = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            isNearChest = false;
        }
    }

    public void OpenChest()
    {
        // Xử lý mở hòm rương
        isChestOpened = true;
        Destroy(gameObject);
    }

    public void OpenChestWithAnimation()
    {
        // Gọi animation của hòm rương
        chestAnimator.SetTrigger("Open"); // Gọi trigger "Open" trong Animator của hòm rương
        isChestOpened = true;

        // Tạo đồng tiền
        StartCoroutine(DropCoins());

        // Thêm 100 coin cho người chơi
        CharacterController player = FindObjectOfType<CharacterController>();
        if (player != null)
        {
            player.collecCoin(100);
            //ShowCoinPopup("+100");
        }
    }

    private IEnumerator DropCoins()
    {
        // Độ cao tối đa mà đồng tiền có thể bắt đầu từ đó
        float maxHeight = 0.5f;
        Vector3 startPos = transform.position;

        for (int i = 0; i < numberOfCoins; i++)
        {
            // Tạo đồng tiền tại một vị trí cao hơn so với vị trí của hòm
            Vector3 coinStartPosition = new Vector3(
                startPos.x + Random.Range(-1f, 1f), // X-offset ngẫu nhiên
                startPos.y + maxHeight, // Y-offset cao hơn
                startPos.z);

            GameObject coin = Instantiate(coinPrefab, coinStartPosition, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Tạo lực ngẫu nhiên để đồng tiền bay lên và ra theo nhiều hướng
                Vector2 direction = new Vector2(Random.Range(-1f, 1f), 1f).normalized; // Tạo vector hướng lên với thành phần x ngẫu nhiên
                float forceMagnitude = Random.Range(3f, 7f); // Điều chỉnh lực để đồng tiền bay lên và ra

                rb.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);

                // Tự hủy đồng tiền sau một khoảng thời gian ngắn
                Destroy(coin, 1f); // Thay đổi giá trị này nếu cần
            }

            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f)); // Khoảng thời gian ngẫu nhiên giữa các đồng tiền bay ra
        }
    }

}
