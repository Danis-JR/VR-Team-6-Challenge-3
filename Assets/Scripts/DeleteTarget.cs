using UnityEngine;

public class DeleteTarget : MonoBehaviour
{
    private string targetType;

    public void specialize(string type)
    {
        targetType = type;
    }

    public void finished()
    {
        //if (targetType == "Ring") { GetComponentInChildren<RingInteractable>().MoveTrigger(); }
    }
    public void delete()
    {
        Destroy(gameObject);
    }
}
