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
        Vector3 currentEuler = transform.rotation.eulerAngles;
        currentEuler.x = 0f;
        currentEuler.z = 0f;
        transform.rotation = Quaternion.Euler(currentEuler);
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
