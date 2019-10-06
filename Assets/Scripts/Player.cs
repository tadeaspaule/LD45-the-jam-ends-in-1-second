﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    public bool inJump = false;
    public GameManager gameManager;
    float originalGravity;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        SwitchToEdit();
    }

    public void SwitchToEdit()
    {
        rb.gravityScale = 0f;
    }

    public void SwitchToGame()
    {
        rb.gravityScale = originalGravity;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name.Equals("end")) {
            gameManager.ResetPositions();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log($"Collided with {other.gameObject.name}, tag {other.gameObject.tag} at {Time.time}");
        if (other.gameObject.name.StartsWith("platform")) {
            inJump = false;
        }
        else if (other.gameObject.tag.Equals("death")) {
            // spikes / bottom of screen / etc
            gameManager.PlayerDied();
            return;
        }
        bool wasEnemy = false;
        Transform check = other.transform;
        while (check.transform.parent != null) {
            if (check.gameObject.GetComponent<EnemyBase>() != null) {
                wasEnemy = true;
                break;
            }
            else {
                check = check.parent;
            }
        }
        if (wasEnemy) {
            Vector3 enemyPos = check.position;
            if (Mathf.Abs(check.position.x-enemyPos.x) < 0.5f && transform.position.y > enemyPos.y) {
                // jumped on the head
                gameManager.EnemyDied(check.gameObject.GetComponent<EnemyBase>());
            }
            else {
                // player died
                gameManager.PlayerDied();
            }
        }
    }

    #region Movement

    public void MoveLeft()
    {
        Move(-1f);
    }

    public void MoveRight()
    {
        Move(1f);
    }

    public void Move(float modifier)
    {
        rb.velocity = new Vector2(0f, rb.velocity.y); // fixes a weird bug where player is dragged to the right
        transform.position += new Vector3(3.5f*modifier*Time.deltaTime,0f,0f);
    }

    public void Jump()
    {
        if (inJump) return;
        rb.AddForce(new Vector2(0f,85f));
        inJump = true;
    }

    #endregion
}
