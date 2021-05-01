using UnityEngine;

public class WildLog : MonoBehaviour, IInteractable
{
    public bool Interact()
    {
        PlanterManager.AddPlanter();
        Destroy(gameObject);
        return true;
    }
}
