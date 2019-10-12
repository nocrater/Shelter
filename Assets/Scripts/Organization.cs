using UnityEngine;
using System.Collections.Generic;

public class Organization : MonoBehaviour
{
    private static Organization instance;

    public List<GameObject> housings;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
