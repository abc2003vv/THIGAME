using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;  // TextMeshProUGUI cho TextMeshPro

    public void UpdateCoinText(int totalCoins)
    {
        if (coinText != null)
        {
            coinText.text += totalCoins;  // Cập nhật UI với số lượng coin
        }
    }

    public void OnWin(int totalCoins)
    {
        // Cập nhật UI khi chiến thắng
        UpdateCoinText(totalCoins);
        // Thực hiện các hành động khác khi người chơi chiến thắng
    }
}
