using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;  

public class GestureTracker : MonoBehaviour
{
    [SerializeField] public GameObject HandMarker;
    [SerializeField] public GameObject snowballPrefab;

    private GameObject snowballInstance;
    private SnowballSpawnable snowballCS;

    public bool isCurledActive = false;
    public bool isUnderHand = true;


    public void GenerateTrigger()
    {
        if (isCurledActive == false)
        { 
           snowballInstance = Instantiate(snowballPrefab, HandMarker.transform.position, HandMarker.transform.rotation);

           snowballCS = snowballInstance.GetComponent<SnowballSpawnable>();
           snowballCS.InitializeObject(gameObject); 

           isCurledActive = true;
        }
    }

    public void ReleaseTrigger()
    {
        if (isCurledActive == true)
        { 
           isCurledActive = false;
        }
    }
}
