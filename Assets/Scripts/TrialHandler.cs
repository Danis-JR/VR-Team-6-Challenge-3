using UnityEngine;

public class TrialHandler : MonoBehaviour
{
    [SerializeField] public TargetHandler targetTracker;

    void Start()
    {
        // Start is called once before the first execution of Update
    }

    void Update()
    {
        // Update is called once per frame
    }

    public void StartTrial()
    {
        targetTracker.StartTrigger();
    }

    public void StopTrial()
    {
        if (targetTracker.remainingTargets == 0)
        {
            // if all are targets hit
        }

        else 
        {
            // if trial ended before all are targets hit
        }
    }
}
