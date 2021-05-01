using UnityEngine;

public class Foragable : MonoBehaviour, IInteractable
{
    public Mushroom mushroom;

    public bool Interact()
    {
        PlanterManager.AddPoints(mushroom.value);
        if (mushroom.plantable)
        {
            PlanterManager.AddDiscoveredMushroom(mushroom);
        }
        Destroy(gameObject);
        return true;
    }
}
