using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;  

public class InfoToggle : MonoBehaviour
{
    [SerializeField] public AudioClip toggleOnSFX;
    [SerializeField] public AudioClip toggleOffSFX;
    private AudioSource audioSource;
    private AudioListener audioListener;

    private GameObject infoDisplay;
    private bool enable = true;

    void Awake()
    {
        infoDisplay = transform.GetChild(0).gameObject;

        audioListener = transform.parent.GetComponent<AudioListener>();
        audioListener.enabled = true;
        AudioListener.volume = 1.0f;
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
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
            audioSource.PlayOneShot(toggleOnSFX);
            infoDisplay.SetActive(true);
            enable = true;
        }

        else
        {
            audioSource.PlayOneShot(toggleOffSFX);
            infoDisplay.SetActive(false);
            enable = false;
        }
    }

}
