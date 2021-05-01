using UnityEngine;

public class LuckStone : MonoBehaviour, IInteractable
{
    public bool Interact()
    {
        Destroy(gameObject);
        return true;
    }
}
