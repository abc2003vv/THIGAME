using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemys : MonoBehaviour
{
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;
    private Transform target;
    private bool facingRight = true;
    private Animator anim;



    void Start()
    {
        target = pointB;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
       

        // Move towards the target
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if reached the target
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            // Change target
            if (target == pointA)
            {
                target = pointB;
            }
            else
            {
                target = pointA;
            }

            // Flip enemy's direction
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
