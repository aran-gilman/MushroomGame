using UnityEngine;

public class RainStone : MonoBehaviour, IInteractable
{
    public bool Interact()
    {
        foreach (Planter.Data data in PlanterManager.PersistentData.planterData)
        {
            data.harvestTime = Time.time;
        }
        Destroy(gameObject);
        return true;
    }
}
