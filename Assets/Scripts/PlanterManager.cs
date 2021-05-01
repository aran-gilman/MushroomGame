using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanterManager : MonoBehaviour
{
    public GameObject planterPrefab;

    public static void AddPlanter()
    {
        data.planterData.Add(new Planter.Data());
    }

    public static void Init()
    {
        data = new Data();
        AddPlanter();
    }

    private class Data
    {
        public List<Planter.Data> planterData = new List<Planter.Data>();
    }
    private static Data data;
    
    private void Start()
    {
        if (data == null)
        {
            Init();
        }

        for (int i = 0; i < data.planterData.Count; i++)
        {
            Transform spot = transform.GetChild(i);
            GameObject planter = Instantiate(planterPrefab, spot);
            planter.GetComponent<Planter>().data = data.planterData[i];
        }
    }
}
