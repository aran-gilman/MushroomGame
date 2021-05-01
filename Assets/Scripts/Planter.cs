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

    public void Interact()
    {
        if (data.mushroom != null)
        {
            if (data.harvestTime <= Time.time)
            {
                PlanterManager.AddPoints(data.mushroom.value);
                data.mushroom = null;
            }
            return;
        }

        data.mushroom = PlanterManager.PersistentData.discoveredMushrooms[0];
    }
}
