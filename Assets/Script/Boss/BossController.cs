using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Animator animator;
    public int health = 100;
    public float attackRange = 5f;
    public float attackInterval = 2f;
    private Transform player;
    private bool isDead = false;
    private float attackCooldown;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        player = GameObject.FindGameObjectWithTag("Character").transform;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= attackCooldown)
        {
            Attack();
            attackCooldown = Time.time + attackInterval;
        }
        else if (!animator.GetBool("isAttacking"))
        {
            Idle();
        }
    }

    void Idle()
    {
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHit", false);
    }

    void Attack()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isHit", false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Death();
        }
        else
        {
            Hit();
        }
    }

    void Hit()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHit", true);
    }

    void Death()
    {
        isDead = true;
        animator.SetBool("isDead", true);
    }
}
