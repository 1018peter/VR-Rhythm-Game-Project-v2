using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{

    public Transform orbitTransformR, orbitTransformG, orbitTransformB;
    
    /// <summary>
    /// The transform that is the parent of all three orbits.
    /// </summary>
    public Transform orbitTransformAll {
        get{
            return orbitTransformR.parent;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
