using System;
using System.Collections;
using UnityEngine;

public class RingInteractable : MonoBehaviour
{
    [SerializeField] public AudioClip spawnSFX;
    [SerializeField] public AudioClip despawnSFX;
    [SerializeField] public AudioClip frozenSFX;

    [SerializeField] private TargetHandler handler;

    [SerializeField] public int targetID = -1;

    private Vector3 minPos;
    private Vector3 maxPos;
    private Vector3 curveOff; 
    private float speed;

    private float timer = 0f;
    private bool isCurve = false;
    private bool isPaused = false;
    private bool isHeard = false;

    private GameObject rim;
    private GameObject origin;
    private Animator animator;

    void Awake()
    {
        rim = transform.parent.gameObject;
        origin = rim.transform.parent.gameObject;
        animator = origin.GetComponent<Animator>();

    }

    void Update()
    {
        if (handler.stayActive(targetID) == false) 
        { 
            if (isHeard == false) 
            { 
                isHeard = true;
            }

            animator.SetTrigger("hit"); 
        }

        if ( isPaused ) 
        {
            if (animator.speed > 0f) { animator.speed -= 0.01f; }
            return;
        }

        timer += Time.deltaTime * speed;
        float t = (1f - Mathf.Cos(timer)) * 0.5f;

        // Place the control point below the midpoint to create a downward U-curve
        if (isCurve == true)
        {
            Vector3 midpoint = (minPos + maxPos) * 0.5f;
            Vector3 controlPos = midpoint + curveOff;

            Vector3 a = Vector3.Lerp(minPos, controlPos, t);
            Vector3 b = Vector3.Lerp(controlPos, maxPos, t);
            origin.transform.position = Vector3.Lerp(a, b, t);
        }

        else { origin.transform.position = Vector3.Lerp(minPos, maxPos, t); }
    }

    public void SetUp(int id, float size, TargetHandler targethandler)
    {
        targetID = id;
        handler = targethandler;
        origin.transform.localScale = new Vector3(size, size, size);
    }

    public void Move(Vector3 mins, Vector3 maxs, Vector3 curve, float moveSpeed)
    {
        minPos.x = origin.transform.position.x - Math.Abs(mins.x);
        minPos.y = origin.transform.position.y - Math.Abs(mins.y);
        minPos.z = origin.transform.position.z - Math.Abs(mins.z);

        maxPos.x = origin.transform.position.x + Math.Abs(maxs.x);
        maxPos.y = origin.transform.position.y + Math.Abs(maxs.y);
        maxPos.z = origin.transform.position.z + Math.Abs(maxs.z);

        curveOff.x = origin.transform.position.x + curve.x;
        curveOff.y = origin.transform.position.y + curve.y;
        curveOff.z = origin.transform.position.z + curve.z;

        speed = moveSpeed;

        if (curve == new Vector3(0f, 0f, 0f)) { isCurve = false; }
        else { isCurve = true; }

        // Find how far along the path (0 to 1) the object currently is
        float totalDist = Vector3.Distance(minPos, maxPos);
        float startT = totalDist > 0f ? Vector3.Distance(minPos, origin.transform.position) / totalDist : 0f;
        startT = Mathf.Clamp01(startT);

        // Reverse your formula: t = (1 - cos(timer)) * 0.5  -->  timer = acos(1 - 2t)
        timer = Mathf.Acos(1f - 2f * startT);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Snowball")
        {

            if (collision.GetComponent<SnowballSpawnable>().isToss == true) 
            {
                handler.HitTrigger(targetID);
                animator.SetTrigger("hit");
            }

            else 
            { 
                StartCoroutine(Pause()); 
            }
        }
    }

    IEnumerator Pause()
    {
        isPaused = true;
        yield return new WaitForSeconds(2.25f);

        isPaused = false;
        animator.speed = 1f;
    }
}
