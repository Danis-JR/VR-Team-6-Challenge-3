using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CourseDisplayer : MonoBehaviour
{
    [SerializeField] TargetHandler courseHandler;
    [SerializeField] private TextMeshProUGUI courseDisplay;
    private Animator animator;
    private bool isFinished = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FinishTrigger() { isFinished = true; }

    public void ChangeFingerTrigger(int gesture)
    {
        if (gesture == 1) { courseDisplay.text = "Course 1"; }

        else if (gesture == 2) { courseDisplay.text = "Course 2"; }

        else if (gesture == 3) { courseDisplay.text = "Course 3"; }

        else if (gesture == 4) { courseDisplay.text = "Course 4"; }

        else { courseDisplay.text = "When Ready"; }
    }

    public void PlayDisplay(string animation)
    {
        if (animation == "start") { animator.SetTrigger("start"); }

        else if (animation == "end") { animator.SetTrigger("end"); }

        else if (animation == "1") { animator.SetTrigger("loopOne"); }

        else if (animation == "2") { animator.SetTrigger("loopTwo"); }

        else if (animation == "3") { animator.SetTrigger("loopThree"); }

        else if (animation == "4") { animator.SetTrigger("loopFour"); }

    }

    public void TriggerInvalid (int request, bool boolean1, bool boolean2, bool boolean3) 
    { 
        StartCoroutine(InvalidRoutine(request, boolean1, boolean2, boolean3)); 
    }

    public IEnumerator InvalidRoutine(int layout, bool playedOne, bool playedTwo, bool playedThree)
    {
        
        if (layout ==  2) { animator.SetTrigger("invalidTwo"); }

        else if (layout == 3) { animator.SetTrigger("invalidThree"); }

        else if (layout == 4) { animator.SetTrigger("invalidFour");  }

        yield return new WaitUntil(() => isFinished == true);

        if (playedOne == false) { animator.SetTrigger("loopOne"); }
        else if (playedTwo == false) { animator.SetTrigger("loopTwo"); }
        else if (playedThree == false) { animator.SetTrigger("loopThree"); }

        isFinished = false;
    }
}
