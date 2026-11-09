using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Hands;

public class SnowballSpawnable : MonoBehaviour
{
    private GestureTracker handObject;
    private Rigidbody physicsObject;

    private bool isReleased = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() { physicsObject = GetComponent<Rigidbody>(); }

    // Update is called once per frame
    void Update()
    {
        if ((handObject.isCurledActive == true) && (isReleased == false))
        {
            physicsObject.isKinematic = true; 
            transform.position = handObject.HandMarker.transform.position;
        }

        else
        {
            physicsObject.isKinematic = false; 
            if (isReleased == false) { isReleased = true; }
        }
    }

    public void InitializeObject(GameObject parent)
    {
        handObject = parent.GetComponent<GestureTracker>();
    }
}
