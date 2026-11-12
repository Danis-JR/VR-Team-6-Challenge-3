using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Hands;

public class SnowballSpawnable : MonoBehaviour
{
    [SerializeField] public GameObject debrisPrefab;
    List<Vector3> trackingPos = new List<Vector3>();

    private GestureTracker parentHand;
    private Rigidbody physicsBody;
    private GameObject debris;

    private bool isReleased = false;
    private bool isToss = true;
    private float velocity = 750f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() 
    { 
        physicsBody = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if ((parentHand.isCurledActive == true) && (isReleased == false))
        {
            physicsBody.useGravity = false; 
            transform.position = parentHand.HandMarker.transform.position;
            transform.rotation = parentHand.HandMarker.transform.rotation;

            if (trackingPos.Count > 10)
            {
                trackingPos.RemoveAt(0);
            }
            trackingPos.Add(transform.position);
        }

        else
        {
            Vector3 direction = trackingPos[trackingPos.Count - 1] - trackingPos[0];
            
            // Wrist Flicking
            if (isToss == false) 
            { 
                velocity = velocity * 0.9f;
                physicsBody.AddForce(direction * velocity); 
            }

            physicsBody.useGravity = true; 
            physicsBody.isKinematic = false; 

            if (isReleased == false) 
            { 
                if (isToss == true) { physicsBody.AddForce(direction * velocity); }
                isReleased = true; 
            }
        }
    }

    public void InitializeObject(GameObject parent)
    {
        parentHand = parent.GetComponent<GestureTracker>();
        isToss = parentHand.isUnderHand;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag != "Ring Target")
        {
           debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
           ParticleSystem particles = debris.GetComponent<ParticleSystem>();
           particles.Play();
           Destroy(gameObject);
        }
    }
}
