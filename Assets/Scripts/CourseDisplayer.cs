using TMPro;
using UnityEngine;

public class CourseDisplayer : MonoBehaviour
{
    [SerializeField] TargetHandler courseHandler;
    [SerializeField] private TextMeshProUGUI courseDisplay;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

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
}
