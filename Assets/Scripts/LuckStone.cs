using UnityEngine;

public class LuckStone : MonoBehaviour, IInteractable
{
    public bool Interact()
    {
        ForestGen.isLuckStoneActive = true;
        Destroy(gameObject);
        return true;
    }
}
