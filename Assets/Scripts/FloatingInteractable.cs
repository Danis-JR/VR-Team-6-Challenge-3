using UnityEngine;

public class FloatingInteractable : MonoBehaviour
{
    private bool hit = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Snowball")
        {
            // Call the hit trigger for the Game Manager  <-
            Destroy(gameObject);
        }
    }
}
