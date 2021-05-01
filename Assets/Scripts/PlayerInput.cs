using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public float speed = 5.0f;
    public Collider2D interactionCollider;

    private Rigidbody2D rb;   

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))
            .normalized * speed;
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
