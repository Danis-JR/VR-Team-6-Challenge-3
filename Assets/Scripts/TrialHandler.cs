using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrialHandler : MonoBehaviour
{
    [SerializeField] public TargetHandler targetTracker;

    //Final Victory Audio Reference
    [SerializeField] public GameAudioManager audioManager;

    // Stores goal times as vectors X = mintues, Y = seconds, z = milliseconds
    [SerializeField]
    private List<Vector3Int> goalTimes = new List<Vector3Int>();
    private List<Vector3Int> PlayerTimes = new List<Vector3Int> {
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, 0)
    };
    private Vector3Int currentTime = new Vector3Int(0, 0, 0);
    private Vector3Int emptyV3I = new Vector3Int(0, 0, 0);

    private List<bool> hasWon = new List<bool> { false, false, false, false };

    public int current_trial = 1;

    public TextMeshProUGUI currentTimeDisplay;
    // references to boards to show the player their completion time
    public TextMeshProUGUI T1TD;
    public TextMeshProUGUI T2TD;
    public TextMeshProUGUI T3TD;
    public TextMeshProUGUI T4TD;

    public bool trialActive = false;


    public void StartTrial()
    {
        targetTracker.StartTrigger();
        trialActive = true;
        StartCoroutine(stopwatch());
    }

    public void StopTrial()
    {
        trialActive = false;

        if (T1TD != null)
        {
            if (current_trial > 0 && current_trial < 4)
            {
                if (validateTime(currentTime, PlayerTimes[current_trial - 1]) || PlayerTimes[current_trial] == emptyV3I)
                {
                    PlayerTimes[current_trial] = currentTime;
                    String newtext = PlayerTimes[current_trial].x.ToString("D2") + ":" + PlayerTimes[current_trial].y.ToString("D2") + ":" + PlayerTimes[current_trial].z.ToString("D2");
                    switch (current_trial)
                    {
                        case 1: T1TD.text = newtext;
                            break;
                        case 2: T2TD.text = newtext;
                            break;
                        case 3: T3TD.text = newtext;
                            break;
                        case 4: T3TD.text = newtext;
                            break;
                    }
                }
            }
        }
        if (validateTime(currentTime, goalTimes[current_trial-1]))
        {

            hasWon[current_trial - 1] = true;
            if (hasWon[0] == true && hasWon[1] == true && hasWon[2] == true && hasWon[3] == true)
            {
                // enter win state
                    if (audioManager != null)
                        audioManager.OnAllLayoutsCompleted();
            }
            else if (current_trial <= 4)
            {
                current_trial ++;
            }

        }
        currentTime.Set(0, 0, 0);

        targetTracker.LayoutTrigger(current_trial);
    }

    private bool validateTime(Vector3Int trialTime, Vector3Int goalTime)
    {
        // compares time of current trial to see if goal was reached

        if (trialTime.x > goalTime.x) { return false; } else if (trialTime.x < goalTime.x) { return true; }
        if (trialTime.y > goalTime.y) { return false; } else if (trialTime.y < goalTime.y) { return true; }
        if (trialTime.z > goalTime.z) { return false; }

        return true;
    }

    public IEnumerator stopwatch()
    {
        float elapsedTime = 0f;

        while (trialActive == true)
        {
            yield return null;

            elapsedTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(elapsedTime);

            currentTime.x = Mathf.RoundToInt(time.Minutes);
            currentTime.y = Mathf.RoundToInt(time.Seconds);
            currentTime.z = Mathf.RoundToInt(time.Milliseconds / 10);

            currentTimeDisplay.text = currentTime.x.ToString("D2") + ":" + currentTime.y.ToString("D2") + ":" + currentTime.z.ToString("D2");
        }
    }
}
