using System;
using System.Linq;
using UnityEngine;

public class Planter : MonoBehaviour, IInteractable
{
    public int index;

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
            mushroomRenderer.sprite = null;
            PlanterManager.PersistentData.planterData[index] = data;
            return true;
        }

        data.mushroom = PlanterManager.PersistentData.discoveredMushrooms
            .Where(m => m.cost <= PlanterManager.PersistentData.totalPoints)
            .OrderByDescending(m => m.value)
            .ElementAt(0);
        data.harvestTime = Time.time + data.mushroom.growTimeInSeconds;
        PlanterManager.RemovePoints(data.mushroom.cost);
        PlanterManager.PersistentData.planterData[index] = data;
        return true;
    }

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer mushroomRenderer;

    private bool CanHarvest() => data.harvestTime < Time.time;
    private bool IsEmpty() => data.mushroom == null;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mushroomRenderer = transform.parent.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsEmpty())
        {
            mushroomRenderer.sprite = data.mushroom.sprite;
            if (CanHarvest())
            {
                spriteRenderer.color = Color.green;
            }
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
