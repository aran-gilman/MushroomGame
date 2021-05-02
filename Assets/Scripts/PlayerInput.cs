﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Direction
{
    Down = 0,
    Left = 1,
    Right = 2,
    Up = 3
}

public class PlayerInput : MonoBehaviour
{
    public float speed = 5.0f;
    public Collider2D interactionCollider;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    private void Update()
    {

        if (Input.GetButtonUp("MainMenu") || PlanterManager.PersistentData.gameOverTime <= Time.time)
        {
            SceneManager.LoadScene("MainMenu");
            // TODO: Add points from the mushrooms that are still growing.
            HighScoreTracker.LastScore = PlanterManager.PersistentData.totalPoints;
            PlanterManager.Init();
            return;
        }

        Vector2 direction =
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (direction.magnitude > 0)
        {
            animator.SetBool("IsWalking", true);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.SetFloat(
                    "Direction", (float)(direction.x > 0 ? Direction.Right : Direction.Left));
            }
            else
            {
                animator.SetFloat(
                    "Direction", (float)(direction.y > 0 ? Direction.Up : Direction.Down));
            }
        }
        else
        {
            audioSource.Stop();
            animator.SetBool("IsWalking", false);
        }
        rb.velocity =  direction * speed;
        if (Input.GetButtonUp("Interact"))
        {
            List<Collider2D> colliders = new List<Collider2D>();
            interactionCollider.OverlapCollider(
                new ContactFilter2D().NoFilter(),
                colliders);
            foreach (Collider2D col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null && interactable.Interact() == true)
                {
                    break;
                }
            }
        }

        if (Input.GetButtonUp("Teleport"))
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == "Forest")
        {
            SceneManager.LoadScene("Garden");
        }
        else if (activeSceneName == "Garden")
        {
            SceneManager.LoadScene("Forest");
        }
    }
}
