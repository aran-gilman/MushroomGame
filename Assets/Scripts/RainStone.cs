using UnityEngine;

public class RainStone : MonoBehaviour, IInteractable
{
    public bool Interact()
    {
        Destroy(gameObject);
        return true;
    }
}
