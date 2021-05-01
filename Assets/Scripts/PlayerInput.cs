using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float speed = 5.0f;
    public Collider2D interactionCollider;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
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
    }
}
