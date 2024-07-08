using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleUI : MonoBehaviour
{
    public static PuzzleUI Instance;

    public GameObject puzzleImage; // Image chứa UI câu hỏi
    public TMP_Text questionText; // Text component để hiển thị câu hỏi
    public Button[] answerButtons; // Mảng các Button cho các đáp án

    private int correctAnswerIndex;
    private Chest currentChest;
    //private bool isVisible = false; // Biến để kiểm tra trạng thái hiển thị của PuzzleUI

    void Awake()
    {
        Instance = this;
        puzzleImage.SetActive(false); // Đảm bảo Image không hiển thị ban đầu
    }

    public void ShowPuzzle(Chest chest, string question, string[] answers, int correctIndex)
    {
        currentChest = chest;
        questionText.text = question;
        correctAnswerIndex = correctIndex;
        //isVisible = true; // Đặt trạng thái hiển thị thành true
        puzzleImage.SetActive(true); // Hiển thị PuzzleUI
        Time.timeScale = 0;//pause

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = answers[i];
            answerButtons[i].onClick.RemoveAllListeners(); // Xóa các sự kiện trước đó
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index)); // Thêm sự kiện mới
        }
    }

    void OnAnswerSelected(int index)
    {
        if (index == correctAnswerIndex)
        {
            currentChest.OpenChestWithAnimation();

        }
        puzzleImage.SetActive(false); // Ẩn PuzzleUI sau khi chọn đáp án
        //isVisible = false; // Đặt trạng thái hiển thị thành false
        OnPuzzleAnsweredCorrectly();
    }

    public void OnPuzzleAnsweredCorrectly()
    {
        // Code to hide the puzzle UI
        Time.timeScale = 1; // Resume the game
    }
}
