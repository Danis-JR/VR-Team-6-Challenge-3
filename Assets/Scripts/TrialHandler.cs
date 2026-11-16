using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrialHandler : MonoBehaviour
{
    [SerializeField] public TargetHandler targetTracker;
    [SerializeField] public SuccessHandler wincheck;

    //Final Victory Audio Reference
    [SerializeField] public GameAudioManager audioManager;

    // Stores goal times as vectors X = mintues, Y = seconds, z = milliseconds
    [SerializeField]
    private List<Vector3Int> goalTimes = new List<Vector3Int>();
    private Vector3Int currentTime = new Vector3Int(0, 0, 0);

    public TextMeshProUGUI timeDisplay;

    public bool trialActive = false;
    
    // Fix — Track the current trial
    [SerializeField] public int current_trial = 1;

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
        if (targetTracker.remainingTargets == 0)
        {
            // if all are targets hit
        }

        if (validateTime(currentTime, goalTimes[current_trial]))
        {
            // Preserve teammate's victory logic
            wincheck.setWin(current_trial);

        }
        else if (targetTracker.remainingTargets != 0)
        {
            // if trial was aborted (May implement, not sure)
        }
        else 
        {
            // if trial ended before all are targets hit
        }

        trialActive = false;
    }

    private bool validateTime(Vector3Int trialTime, Vector3Int goalTime)
    {
        // compares time of current trial to see if goal was reached

        if (trialTime.x > goalTime.x) { return false; }
        if (trialTime.y > goalTime.y) { return false; }
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
    }
}
