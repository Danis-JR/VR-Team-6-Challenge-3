using UnityEngine;

public class FloorInteractable : MonoBehaviour
{
    [SerializeField] private TargetHandler handler;
    public int points = 0;
    // Update is called once per frame

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Snowball")
        {
            handler.validHit(points);
        }
    }
}
