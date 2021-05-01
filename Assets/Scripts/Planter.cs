using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public Mushroom mushroom;
        public float harvestTime;
    }
    public Data data;
}
