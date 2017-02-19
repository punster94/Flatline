using UnityEngine;
using System.Collections;

public class WallSegmentBehavior : MonoBehaviour
{
    private float defaultDuration = 1f;
    private float duration;
    private float startTime;

	// Use this for initialization
	void Start ()
    {
        duration = defaultDuration;
        startTime = Time.time;
	}

    public void setDuration(float d)
    {
        duration = d;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time - startTime >= duration)
            Destroy(gameObject);
	}
}
