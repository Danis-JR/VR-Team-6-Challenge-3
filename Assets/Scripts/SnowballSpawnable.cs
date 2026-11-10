using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Hands;

public class SnowballSpawnable : MonoBehaviour
{
    List<Vector3> trackingPos = new List<Vector3>();

    private GestureTracker parentHand;
    private Rigidbody physicsBody;

    private bool isReleased = false;
    private float velocity = 1000f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() 
    { 
        physicsBody = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if ((parentHand.isCurledActive == true) && (isReleased == false))
        {
            physicsBody.useGravity = false; 
            transform.position = parentHand.HandMarker.transform.position;
            transform.rotation = parentHand.HandMarker.transform.rotation;

            if (trackingPos.Count > 5)
            {
                trackingPos.RemoveAt(0);
            }
            trackingPos.Add(transform.position);
        }

        else
        {
            Vector3 direction = trackingPos[trackingPos.Count - 1] - trackingPos[0];
            
            // Wrist Flicking
            if (parentHand.isUnderHand == false) { physicsBody.AddForce(direction * velocity); }

            physicsBody.useGravity = true; 
            physicsBody.isKinematic = false; 

            if (isReleased == false) 
            { 
                if (parentHand.isUnderHand == true) { physicsBody.AddForce(direction * velocity); }
                isReleased = true; 
            }
        }
    }

    public void InitializeObject(GameObject parent)
    {
        parentHand = parent.GetComponent<GestureTracker>();
    }
}
