using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    // [SerializeField] private GameManager gameTracker;

    public bool gameStart = false;
    public int remainingTargets = 0;
    private int initialCount = 0;

    void Awake() 
    { 
        initialCount = remainingTargets;
    }

    public void reset() 
    { 
        remainingTargets = initialCount; 
        gameStart = false;
    }

    public void HitTrigger(string targetID)
    {
        if (remainingTargets > 0) { remainingTargets--; }

        // if (remainingTargets == 0) { gameTracker.endSequence(); }
    }

    public void StartTrigger()
    {
        gameStart = true;
    }

}
