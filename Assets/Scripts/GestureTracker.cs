using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;  

public class GestureTracker : MonoBehaviour
{
    [SerializeField] public AudioClip underhandSummonSFX;
    [SerializeField] public AudioClip underhandThrowSFX;
    [SerializeField] public AudioClip wristSummonSFX;
    [SerializeField] public AudioClip wristThrowSFX;

    [SerializeField] public GameObject HandMarker;
    [SerializeField] public GameObject snowballPrefab;

    private GameObject snowballInstance;
    private SnowballSpawnable snowballCS;
    private AudioSource audioSource;

    public bool isCurledActive = false;
    public bool isUnderHand = true;

    void Awake() {}


    public void GenerateTrigger()
    {
        if (isCurledActive == false)
        { 
           //if (isUnderHand == true) { audioSource.PlayOneShot(underhandSummonSFX); }
           //else { audioSource.PlayOneShot(wristSummonSFX); }

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

    public void UnderHandTrigger()
    {
        if (isUnderHand == false)
        { 
           //audioSource.PlayOneShot(underhandThrowSFX);
           isUnderHand = true; 
        }
    }

    public void WristThrowTrigger()
    {
        if (isUnderHand == true)
        { 
           //audioSource.PlayOneShot(wristThrowSFX);
           isUnderHand = false; 
        }
    }
}
