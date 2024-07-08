using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    public ParticleSystem deathParticleSystem;

    public void PlayVFX(Vector3 position)
    {
        transform.position = position;
        deathParticleSystem.Play();
    }
}
