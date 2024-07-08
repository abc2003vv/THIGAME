using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVFX : MonoBehaviour
{
    public ParticleSystem coinParticleSystem;

    void Start()
    {
        // Thêm cách để gọi hàm PlayVFX tại thời điểm người chơi nhận được coin
    }

    public void PlayVFX(Vector3 position)
    {
        transform.position = position;
        coinParticleSystem.Play();
    }
}
