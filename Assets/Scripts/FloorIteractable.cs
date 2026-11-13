using UnityEngine;

public class FloorInteractable : MonoBehaviour
{
    [SerializeField] private TargetHandler handler;
    [SerializeField] public int targetID = -1;

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
        if (handler.stayActive(targetID) == false) { animator.SetTrigger("hit"); }
    }

    public void setUp(int id, TargetHandler targethandler)
    {
        targetID = id;
        handler = targethandler;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Snowball")
        {
            handler.HitTrigger(targetID);
            animator.SetTrigger("hit");
        }
    }
}
