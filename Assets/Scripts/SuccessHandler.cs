using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SuccessHandler : MonoBehaviour
{
    [SerializeField] public TrialHandler trialhandler;

    public List<bool> wins = new List<bool> { false, false, false, false };

    public void setWin (int layout)
    {
        wins[layout - 1] = true;

        if (wins[0] == true && wins[1] == true && wins[2] == true && wins[3] == true)
        {
            // tell game to enter 'win' state and do whatever it's meant to do when the player succeeds.
            if (trialhandler.audioManager != null)
                trialhandler.audioManager.OnAllLayoutsCompleted();
        }
    }
}
