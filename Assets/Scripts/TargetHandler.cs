using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    private int score = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void validHit (int points)
    {
        score += points;
    }
}
