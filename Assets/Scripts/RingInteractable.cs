using UnityEngine;

public class RingInteractable : MonoBehaviour
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
        if (handler.gameStart == false) { triggered = false; }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (triggered == false)
        {
            if (collision.gameObject.tag == "Snowball")
            {
                animator.SetTrigger("spin");

                if (handler.gameStart == true)
                {
                    handler.HitTrigger(targetID);
                    triggered = true;
                }
            }
        }
    }
}
