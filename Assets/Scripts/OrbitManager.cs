using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    
    Quaternion initialRotationB = Quaternion.Euler(-90, 0, 0);
    Quaternion initialRotationR = Quaternion.Euler(0, 0, 0);
    Quaternion initialRotationG = Quaternion.Euler(0, 90, 0);


    public IEnumerator RotateAllToInitial(float timeInSeconds, Action callback){
        float t = 0;
        Quaternion startRotationR = orbitTransformR.localRotation;
        Quaternion startRotationG = orbitTransformG.localRotation;
        Quaternion startRotationB = orbitTransformB.localRotation;
        while(t < timeInSeconds){
            orbitTransformR.localRotation = Quaternion.Lerp(startRotationR, initialRotationR, t);
            orbitTransformG.localRotation = Quaternion.Lerp(startRotationG, initialRotationG, t);
            orbitTransformB.localRotation = Quaternion.Lerp(startRotationB, initialRotationB, t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        orbitTransformR.localRotation = initialRotationR;
        orbitTransformG.localRotation = initialRotationG;
        orbitTransformB.localRotation = initialRotationB;
        Debug.Log("RotateAllToInitial done");

        
        callback();
    }

    public IEnumerator RotateAllToSideways(float timeInSeconds, Action callback){
        float t = 0;
        Quaternion startRotationR = orbitTransformR.localRotation;
        Quaternion startRotationG = orbitTransformG.localRotation;
        Quaternion startRotationB = orbitTransformB.localRotation;
        while(t < timeInSeconds){
            orbitTransformR.localRotation = Quaternion.Lerp(startRotationR, initialRotationB, t);
            orbitTransformG.localRotation = Quaternion.Lerp(startRotationG, initialRotationB, t);
            orbitTransformB.localRotation = Quaternion.Lerp(startRotationB, initialRotationB, t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        orbitTransformR.localRotation = initialRotationB;
        orbitTransformG.localRotation = initialRotationB;
        orbitTransformB.localRotation = initialRotationB;
        Debug.Log("RotateAllToSideways done");
        callback();

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
