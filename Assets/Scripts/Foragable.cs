using UnityEngine;

public class Foragable : MonoBehaviour, IInteractable
{
    public Mushroom mushroom;

    public void Interact()
    {
        PlanterManager.AddPoints(mushroom.value);
        if (mushroom.plantable)
        {
            PlanterManager.AddDiscoveredMushroom(mushroom);
        }
    }
}
