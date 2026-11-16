using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrialHandler : MonoBehaviour
{
    [SerializeField] public TargetHandler targetTracker;

    // Stores goal times as vectors X = mintues, Y = seconds, z = milliseconds
    [SerializeField]
    private List<Vector3Int> goalTimes = new List<Vector3Int>();
    private Vector3Int currentTime = new Vector3Int(0, 0, 0);

    public int current_trial = 1;

    public TextMeshProUGUI timeDisplay;

    public bool trialActive = false;

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
        trialActive = true;
        StartCoroutine(stopwatch());
    }

    public void StopTrial()
    {
        trialActive = false;
        if (current_trial < 4)
        {
            if (validateTime(currentTime, goalTimes[current_trial]))
            {
                current_trial += 1;
            }
            currentTime.Set(0, 0, 0);
            targetTracker.LayoutTrigger(current_trial);
        }


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

            timeDisplay.text = currentTime.x.ToString("D2") + ":" + currentTime.y.ToString("D2") + ":" + currentTime.z.ToString("D2");
        }

        currentTime.Set(0, 0, 0);
    }
}
