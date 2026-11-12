using UnityEngine;

public class FloorInteractable : MonoBehaviour
{
    [SerializeField] private TargetHandler handler;
    [SerializeField] private string targetID;

    private GameObject rim;
    private GameObject origin;
    private Animator animator;

    private bool triggered = false;
    
    void Awake()
    {
        rim = transform.parent.gameObject;
        origin = rim.transform.parent.gameObject;
        animator = origin.GetComponent<Animator>();
    }

    void Update()
    {
        if (handler.trialActive == false) { triggered = false; }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (triggered == false)
        {
            if (collision.gameObject.tag == "Snowball")
            {
                animator.SetTrigger("relative");

                if (handler.trialActive == true)
                {
                    handler.HitTrigger(targetID);
                    triggered = true;
                }
            }
        }
    }
}
