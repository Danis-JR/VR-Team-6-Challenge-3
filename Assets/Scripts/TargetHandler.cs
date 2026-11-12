using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    [SerializeField] public TrialHandler trialTracker;

    public int remainingTargets = 0;
    private int initialCount = 0;
    public bool trialActive = false;

    void Awake() 
    { 
        initialCount = remainingTargets;
    }

    public void reset() 
    { 
        remainingTargets = initialCount; 
        trialActive = false;
    }

    public void HitTrigger(string targetID)
    {
        if (remainingTargets > 0) { remainingTargets--; }

        if (remainingTargets == 0) { trialTracker.StopTrial(); }
    }

    public void StartTrigger()
    {
        trialActive = true;
    }

}
