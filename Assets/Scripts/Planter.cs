using System;
using System.Linq;
using UnityEngine;

public class Planter : MonoBehaviour, IInteractable
{
    [Serializable]
    public class Data
    {
        public Mushroom mushroom;
        public float harvestTime;
    }
    public Data data;

    public bool Interact()
    {
        if (!IsEmpty())
        {
            if (!CanHarvest())
            {
                return false;
            }
            PlanterManager.AddPoints(data.mushroom.value);
            data.mushroom = null;
            return true;
        }

        data.mushroom = PlanterManager.PersistentData.discoveredMushrooms
            .Where(m => m.cost <= PlanterManager.PersistentData.totalPoints)
            .OrderByDescending(m => m.value)
            .ElementAt(0);
        data.harvestTime = Time.time + data.mushroom.growTimeInSeconds;
        PlanterManager.RemovePoints(data.mushroom.cost);
        return true;
    }

    private SpriteRenderer spriteRenderer;

    private bool CanHarvest() => data.harvestTime < Time.time;
    private bool IsEmpty() => data.mushroom == null;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (IsEmpty())
        {
            spriteRenderer.color = Color.red;
        }
        else if (CanHarvest())
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
