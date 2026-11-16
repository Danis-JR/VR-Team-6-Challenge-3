using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;  

public class GestureTracker : MonoBehaviour
{
    [SerializeField] public GameAudioManager audioManager;
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
    int canSpeak = 0;

    void Awake() 
    { 
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.volume = 0.7f;
    }


    public void GenerateTrigger()
    {
        if (isCurledActive == false)
        { 
           if (isUnderHand == true) { audioSource.PlayOneShot(underhandSummonSFX); }
           else { audioSource.PlayOneShot(wristSummonSFX); }

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
           canSpeak++;
           if (canSpeak == UnityEngine.Random.Range(10, 13)) { audioManager.OnSnowballThrown(); canSpeak = 0; }
           isCurledActive = false;
        }
    }

    public void UnderHandTrigger()
    {
        if (isUnderHand == false)
        { 
           audioSource.PlayOneShot(underhandThrowSFX);
           isUnderHand = true; 
        }
    }

    public void WristThrowTrigger()
    {
        if (isUnderHand == true)
        { 
           audioSource.PlayOneShot(wristThrowSFX);
           isUnderHand = false; 
        }
    }
}
