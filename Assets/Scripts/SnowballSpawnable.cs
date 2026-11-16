using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Hands;


public struct TrackedFrame 
{
    public Vector3 position;
    public float time;
}

public class SnowballSpawnable : MonoBehaviour
{
    [SerializeField] public GameObject debrisPrefab;
    [SerializeField] private Material normalSnow;
    [SerializeField] private Material frostedSnow;

    //List<Vector3> trackingPos = new List<Vector3>();
    private List<TrackedFrame> trackingFrames = new List<TrackedFrame>();

    private GestureTracker parentHand;
    private Rigidbody physicsBody;
    private Animator animator;
    private GameObject debris;

    private bool isReleased = false;
    public bool isToss = true;

    private float throwVelocity = 2f;
    private float flingVelocity = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() 
    { 
        physicsBody = GetComponent<Rigidbody>(); 
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((parentHand.isCurledActive == true) && (isReleased == false))
        {
            physicsBody.useGravity = false; 
            if (isToss == false) { physicsBody.isKinematic = false; }

            transform.position = parentHand.HandMarker.transform.position;
            transform.rotation = parentHand.HandMarker.transform.rotation;

            if (trackingFrames.Count > 5)
            {
                trackingFrames.RemoveAt(0);
            }

            trackingFrames.Add(new TrackedFrame {position = transform.position, time = Time.fixedTime });
        }

        else if (isReleased == false)
        {
            OnReleaseTrigger();
        }
    }

    private void OnReleaseTrigger()
    {
        if (trackingFrames.Count < 2) return;

        TrackedFrame oldest = trackingFrames[0];
        TrackedFrame newest = trackingFrames[trackingFrames.Count - 1];

        float timeElapsed = newest.time - oldest.time;
        if (timeElapsed <= 0f) return;

        Vector3 direction = (newest.position - oldest.position) / timeElapsed;

        physicsBody.useGravity = true; 
        physicsBody.isKinematic = false; 
            
        // Wrist Flicking
        if (isToss == false) { physicsBody.AddForce(direction.normalized * flingVelocity, ForceMode.Impulse); }

        else { physicsBody.AddForce(direction * throwVelocity, ForceMode.Impulse);}

        isReleased = true; 
        // Notify Audio Manager When Snowball Is Thrown
        GameAudioManager audioManager = FindFirstObjectByType<GameAudioManager>();

        if (audioManager != null)
        {
            audioManager.OnSnowballThrown();
        }

    }

    public void InitializeObject(GameObject parent)
    {
        parentHand = parent.GetComponent<GestureTracker>();
        animator.SetBool("isUnderhand", parentHand.isUnderHand);
        isToss = parentHand.isUnderHand;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag != "Ring Target")
        {
           debris = Instantiate(debrisPrefab, transform.position, transform.rotation);

           ParticleSystem particles = debris.GetComponent<ParticleSystem>();
           ParticleSystemRenderer render = debris.GetComponent<ParticleSystemRenderer>();

           if (isToss == true) { render.material = normalSnow; }
           else { render.material = frostedSnow; }
           particles.Play();

           Destroy(gameObject);
        }
    }
}
