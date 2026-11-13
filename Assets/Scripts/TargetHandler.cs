using System; // <-- Add this line
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Target
{
    public GameObject spawnAnchor;
    public string type;
    public int id;
    public bool isActive;
}

public class TargetHandler : MonoBehaviour
{
    [SerializeField] public GameObject RingTargetPrefab;
    [SerializeField] public GameObject FloorTargetPrefab;
    [SerializeField] public TrialHandler trialTracker;

    [SerializeField]
    private List<Target> first_layout = new List<Target>();

    [SerializeField]
    private List<Target> second_layout = new List<Target>();

    [SerializeField]
    private List<Target> third_layout = new List<Target>();

    [SerializeField]
    private List<Target> fourth_layout = new List<Target>();

    private List<Target> current_layout = new List<Target>();

    //private int current_target = 0;
    public int remainingTargets = 0;
    public bool trialActive = false;

    void Awake() 
    { 
        current_layout = first_layout;
        //current_target = -1;
    }

    public void reset() 
    { 
        trialActive = false;
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
        StartCoroutine(spawnTargets());
    }

    public bool stayActive(int targetID)
    {
        return current_layout[targetID].isActive;
    }


    IEnumerator spawnTargets()
    {
        remainingTargets = current_layout.Count;

        for (int i = 0; i < current_layout.Count; i++)
        {
            if (current_layout[i].isActive == false)
            {
                if ( current_layout[i].type == "Ring")
                {
                    GameObject target = Instantiate(RingTargetPrefab, current_layout[i].spawnAnchor.transform.position, current_layout[i].spawnAnchor.transform.rotation);
                    target.GetComponentInChildren<RingInteractable>().setUp(i, this);
                }

                else if ( current_layout[i].type == "Floor")
                {
                    GameObject target = Instantiate(FloorTargetPrefab, current_layout[i].spawnAnchor.transform.position, current_layout[i].spawnAnchor.transform.rotation);
                    target.GetComponentInChildren<FloorInteractable>().setUp(i, this);
                }

                current_layout[i].isActive = true;

                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    IEnumerator clearTargets()
    {
        remainingTargets = 0;

        for (int i = 0; i < current_layout.Count; i++)
        {
            if (current_layout[i].isActive == true)
            {
                current_layout[i].isActive = false;
                yield return new WaitForSeconds(1f);
            }
        }
    }


}
