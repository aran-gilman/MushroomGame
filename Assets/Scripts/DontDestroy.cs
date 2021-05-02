using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        List<DontDestroy> objs =
            FindObjectsOfType<DontDestroy>()
            .Where(x => x.gameObject.name == gameObject.name)
            .ToList();
        if (objs.Count > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
