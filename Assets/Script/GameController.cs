using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageInterval = 0.5f; // Interval between damage ticks

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            StartCoroutine(ApplyContinuousDamage(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            StopAllCoroutines(); // Stop applying damage when the character leaves the trap
        }
    }

    private IEnumerator ApplyContinuousDamage(Collider2D character)
    {
        CharacterController characterController = character.GetComponent<CharacterController>();
        if (characterController != null)
        {
            while (true)
            {
                characterController.TakeDamage(damageAmount);
                characterController.TriggerHitEffect();
                yield return new WaitForSeconds(damageInterval);
            }
        }
    }
}
