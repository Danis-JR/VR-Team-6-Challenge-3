using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct moveCurve
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public struct moveRange
{
    public float min;
    public float max;
}

[Serializable]
public class Target
{
    [HideInInspector] public bool isActive;
    private GameObject instance;

    [Header("Orientation & Type")]
    public GameObject spawnAnchor;
    public string type;

    [Header("Speed & Curvature")]
    public float speed;
    public moveCurve curve;

    [Header("Size")]
    public float size = 1f;

    [Header("Position Offsets")]
    public moveRange xRange;
    public moveRange yRange;
    public moveRange zRange;

    public void reserve(GameObject target) { instance = target; }
    public bool isEmpty() 
    { 
        if (instance == null) { return true; }
        else{ return false; }
    }
}

public class TargetHandler : MonoBehaviour
{
    [SerializeField] public GameObject RingTargetPrefab;
    [SerializeField] public GameObject FloorTargetPrefab;
    [SerializeField] public GameObject BarrierPrefab;
    [SerializeField] public TrialHandler trialTracker;
    [SerializeField] public CourseDisplayer courseSign;

    [SerializeField]
    private List<Target> first_layout = new List<Target>();

    [SerializeField]
    private List<Target> second_layout = new List<Target>();

    [SerializeField]
    private List<Target> third_layout = new List<Target>();

    [SerializeField]
    private List<Target> fourth_layout = new List<Target>();

    private List<Target> current_layout = new List<Target>();

    public int remainingTargets = 0;
    public bool trialActive = false;

    private int activeLayout = 1;
    private bool activeSetup = false;

    void Awake() 
    { 
        activeSetup = true;
        current_layout = first_layout;
        StartCoroutine(SpawnTargets());
    }

    public bool stayActive(int targetID)
    {
        return current_layout[targetID].isActive;
    }


    public void LayoutTrigger(int layout) 
    { 
        if ((trialActive == false) && (activeSetup == false))
        {
            activeSetup = true;
            StartCoroutine(LayoutRoutine(layout));
        }
    }

    public void HitTrigger(int targetID)
    {
        if (remainingTargets > 0) { remainingTargets--; }

        if (remainingTargets == 0) { if (trialActive) { trialTracker.StopTrial(); } }

        current_layout[targetID].isActive = false;
    }

    public void StartTrigger()
    {
        trialActive = false;
        courseSign.PlayDisplay("start");
    }

    IEnumerator SpawnTargets()
    {
        remainingTargets = 0;

        for (int i = 0; i < current_layout.Count; i++)
        {
            if (current_layout[i].isActive == false)
            {
                GameObject target = null;
                current_layout[i].isActive = true;
                current_layout[i].spawnAnchor.SetActive(true);

                Vector3 minVals = new Vector3(current_layout[i].xRange.min, current_layout[i].yRange.min, current_layout[i].zRange.min);
                Vector3 maxVals = new Vector3(current_layout[i].xRange.max, current_layout[i].yRange.max, current_layout[i].zRange.max);
                Vector3 curveVal = new Vector3(current_layout[i].curve.x, current_layout[i].curve.y, current_layout[i].curve.z);
                

                if ( current_layout[i].type == "Ring")
                {
                    if (current_layout[i].size == 0f) { current_layout[i].size = 1f; }
                    target = Instantiate(RingTargetPrefab, current_layout[i].spawnAnchor.transform.position, current_layout[i].spawnAnchor.transform.rotation);
                    target.GetComponentInChildren<RingInteractable>().SetUp(i, current_layout[i].size, this);
                    target.GetComponentInChildren<RingInteractable>().Move(minVals, maxVals, curveVal, current_layout[i].speed);
                    remainingTargets++;
                }

                else if ( current_layout[i].type == "Floor")
                {
                    if (current_layout[i].size == 0f) { current_layout[i].size = 1f; }
                    target = Instantiate(FloorTargetPrefab, current_layout[i].spawnAnchor.transform.position, current_layout[i].spawnAnchor.transform.rotation);
                    target.GetComponentInChildren<FloorInteractable>().SetUp(i, current_layout[i].size, this);
                    target.GetComponentInChildren<FloorInteractable>().Move(minVals, maxVals, curveVal, current_layout[i].speed);
                    remainingTargets++;
                }

                else 
                {
                    if (current_layout[i].size == 0f) { current_layout[i].size = 1f; }
                    target = Instantiate(BarrierPrefab, current_layout[i].spawnAnchor.transform.position, current_layout[i].spawnAnchor.transform.rotation);
                    target.GetComponentInChildren<BarrierInteractable>().SetUp(i, current_layout[i].size, this);
                    target.GetComponentInChildren<BarrierInteractable>().Move(minVals, maxVals, curveVal, current_layout[i].speed);
                }

                current_layout[i].reserve(target);

                yield return new WaitForSeconds(0.3f);
            }
        }

        activeSetup = false;
    }

    IEnumerator ClearTargets()
    {
        remainingTargets = -1;

        for (int i = 0; i < current_layout.Count; i++)
        {
            current_layout[i].isActive = false;
            current_layout[i].spawnAnchor.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitUntil(() => current_layout[current_layout.Count - 1].isEmpty() == true);
    }

    IEnumerator LayoutRoutine(int layout)
    {
        if (activeLayout != layout) { yield return StartCoroutine(ClearTargets()); }

        if (layout == 1) 
        { 
            activeLayout = 1;
            current_layout = first_layout; 
        }
        else if (layout == 2) 
        {
            activeLayout = 2; 
            current_layout = second_layout; 
        }
        else if (layout == 3) 
        { 
            activeLayout = 3;
            current_layout = third_layout; 
        }
        else
        { 
            activeLayout = 4;
            current_layout = fourth_layout; 
        }

        courseSign.PlayDisplay(activeLayout.ToString());
        StartCoroutine(SpawnTargets());
    }
}
