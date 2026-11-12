using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;  

public class InfoToggle : MonoBehaviour
{
    private GameObject infoDisplay;
    private bool enable = false;

    void Awake()
    {
        infoDisplay = transform.GetChild(0).gameObject;
        infoDisplay.SetActive(false);
    }

    void LateUpdate()
    {
        Vector3 horizontalForward = Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up);
        transform.rotation = Quaternion.LookRotation(horizontalForward.normalized, Vector3.up);
    }

    public void EnableTrigger()
    {
        if (enable == false)
        { 
           infoDisplay.SetActive(true);
           enable = true;
        }

        else
        {
           infoDisplay.SetActive(false);
           enable = false;
        }
    }

}
