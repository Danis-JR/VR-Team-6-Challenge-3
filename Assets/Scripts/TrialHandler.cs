using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrialHandler : MonoBehaviour
{
    [SerializeField] public TargetHandler targetTracker;

    public string goal_time = "";
    public string current_time = "";
    public TextMeshProUGUI watchDisplay;

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
        if (targetTracker.remainingTargets == 0)
        {
            // if all are targets hit
        }

        else 
        {
            // if trial ended before all are targets hit
        }

        trialActive = false;
    }

    public IEnumerator stopwatch()
    {
        float elapsedTime = 0f;

        while (trialActive == true)
        {
            yield return null;

            elapsedTime += Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(elapsedTime);

            current_time = time.Minutes.ToString("D2") + ":" + time.Seconds.ToString("D2") + ":" + (time.Milliseconds / 10).ToString("D2");

            //current_time = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);

            watchDisplay.text = current_time;
        }
    }
}
