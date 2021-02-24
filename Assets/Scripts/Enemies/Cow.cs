﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : EnemyController
{
    [Header("Movement")]
    public float stoppingDis;
    public float normalSpeed;               // Normal speed
    public float madSpeed;                  // Mad speed
    float speed;                            // Move speed

    [Header("Attack")]
    public float sightDistance;
    public float returnNormalTime;          // Time for turning back to normal when player out of sight
    [SerializeField] float returnNormalTimer;                // Timer
    [SerializeField] bool playerInSight = false;             // True when player in sight
    [SerializeField] bool isMad = false;
    bool hitPlayer = false;
    bool isDashing = false;

    // Component
    GameObject player;
    SightController sightController;
    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sightController = GetComponent<SightController>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();            // Cow animator

        
    }

    void Update()
    {
        playerInSight = (sightController.PlayerInSight(Vector2.right, sightDistance) || sightController.PlayerInSight(Vector2.left, sightDistance));
        int dir = MoveDir();

        if (playerInSight)
        {
            isMad = true;
            returnNormalTimer = returnNormalTime;
        }

        if (isMad)        // Dash to player
        {
            speed = madSpeed;

            if (playerInSight)
            {
                if (!hitPlayer)
                {
                    anim.SetTrigger("Attack");
                    isDashing = true;
                    rb.velocity = new Vector2(dir * speed, rb.velocity.y);        // Run to player
                }
            }
            else
            {
                if (isDashing)
                    StopDash();
                
                // Player outside of sight
                if (returnNormalTimer <= 0)
                {
                    returnNormalTimer = returnNormalTime;
                    isMad = false;
                }
                else
                {
                    returnNormalTimer -= Time.deltaTime;
                }
            }
        }
        else
        {
            speed = normalSpeed;
        }

        anim.SetBool("IsDashing", isDashing);
        anim.SetBool("IsMad", isMad);
    }

    int MoveDir()
    {
        float dis = player.transform.position.x - transform.position.x;

        if (-stoppingDis < dis && dis < stoppingDis)
            return 0;

        if (player.transform.position.x > transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            return 1;
        }
        else //(player.transform.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            return -1;
        }
    }

    void StopDash()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        hitPlayer = true;
        isDashing = false;
        StartCoroutine(AttackDelay());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit player
        if(collision.transform.tag == "Player")
        {
            StopDash();
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(2);
        hitPlayer = false;
    }

    private void OnDrawGizmos()
    {
        // Sight
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + sightDistance, transform.position.y, transform.position.z));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - sightDistance, transform.position.y, transform.position.z));
    }
}
