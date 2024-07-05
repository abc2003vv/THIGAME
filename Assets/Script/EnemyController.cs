using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;
    private Animator anim;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    // Method to handle enemy taking damage
    public void TakeDamage()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
        // Trigger Death animation
        anim.SetTrigger("isDeath");
        // Disable enemy's interactions
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }
}
