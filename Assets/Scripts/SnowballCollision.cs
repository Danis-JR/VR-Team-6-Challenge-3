using UnityEngine;

public class SnowballCollision : MonoBehaviour
{

    [SerializeField] private SnowballSpawnable parent;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Face Plate")
        {
            //parent.DestoryObject();
        }
    }
}
