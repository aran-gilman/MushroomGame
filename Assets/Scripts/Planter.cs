using System;
using System.Collections;
using System.Collections.Generic;
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
        if (data.mushroom != null)
        {
            if (data.harvestTime > Time.time)
            {
                return false;
            }
            PlanterManager.AddPoints(data.mushroom.value);
            data.mushroom = null;
            return true;
        }

        data.mushroom = PlanterManager.PersistentData.discoveredMushrooms[0];
        data.harvestTime = Time.time + data.mushroom.growTimeInSeconds;
        return true;
    }
}
