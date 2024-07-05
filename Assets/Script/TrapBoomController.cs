using UnityEngine;
using System.Collections;

public class TrapBoomController : MonoBehaviour
{
    public int explosionDamage = 10; // Damage dealt by the explosion to characters
    public float explosionRadius = 2f; // Radius of the explosion effect
    public LayerMask targetLayer; // LayerMask to filter targets affected by the explosion
    private Animator anim; // Animator component for handling animations

    private bool isTriggered = false; // Flag to check if the trap is triggered

    void Start()
    {
        // Get the Animator component attached to the GameObject
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the trap is triggered and the colliding object is a character
        if (!isTriggered && other.CompareTag("Character"))
        {
            isTriggered = true; // Mark the trap as triggered
            StartCoroutine(BoomDelay()); // Start the explosion delay coroutine
        }
    }

    IEnumerator BoomDelay()
    {
        // Trigger the explosion animation if an animator is available
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }

        yield return new WaitForSeconds(1f); // Wait for 1 second before executing boom

        ExecuteBoom(); // Execute the explosion
    }

    // Method to be called by the animation event
    public void ExecuteBoom()
    {
        // Detect all colliders within the explosion radius that match the target layer
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);

        // Iterate through the detected colliders
        foreach (Collider2D col in colliders)
        {
            // Destroy objects tagged as "Box" or "Enemy"
            if (col.CompareTag("Box") || col.CompareTag("Enemy"))
            {
                Destroy(col.gameObject); // Destroy the box or enemy
            }
            // Deal damage to objects tagged as "Character"
            else if (col.CompareTag("Character"))
            {
                CharacterController character = col.GetComponent<CharacterController>();
                if (character != null)
                {
                    character.TakeDamage(explosionDamage); // Reduce character's health by explosionDamage
                }
            }
        }

        isTriggered = false;

        // Destroy the trap gameObject after the explosion
        Destroy(gameObject, 1f); // Adjust the delay to allow animation to play if needed
    }

    // Draw the explosion radius in the Unity Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Set the color of the Gizmos
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Draw the wireframe sphere
    }
}
