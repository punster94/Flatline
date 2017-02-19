using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{
    private bool mouseWasDown;
    private Vector3 startPosition;
    private Vector3 currentPosition;
    private float powerupTimer;
    private float currentDist;
    private float initialMaxDist;
    private float timeHeldDown;
    private float timeToExplode = .5f;

    public Transform wall;
    public Transform parent;
    public float maxDistance;
    public float smoothness;
    public Transform background;
    public Transform player;
    public float lineDuration;
    public float powerupTime;
    public bool canDestroyWave;
    public bool gotFirstExplosion;
    public bool gotSecondExplosion;

    public Transform bulletBucket;
    public Transform enemyBucket;
    public Transform animBucket;
    public GameObject impulse;
    public AudioController audioController;

    // Use this for initialization
    void Start()
    {
        mouseWasDown = false;
        initialMaxDist = maxDistance;
        canDestroyWave = false;
        gotFirstExplosion = false;
        gotSecondExplosion = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canDestroyWave)
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && heartPressed(Input.GetTouch(0).position))
                {
                    timeHeldDown += Time.deltaTime;
                    if(timeHeldDown >= timeToExplode)
                    {
                        explodeHeart();
                    }
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    timeHeldDown = 0;
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && heartPressed(Input.mousePosition))
                {
                    timeHeldDown += Time.deltaTime;
                    if (timeHeldDown >= timeToExplode)
                    {
                        explodeHeart();
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    timeHeldDown = 0;
                }
            }
        }


        if (powerupTimer > 0)
        {
            powerupTimer -= Time.deltaTime;
            if (powerupTimer <= 0)
            {
                maxDistance = initialMaxDist;
            }
        }

        if (Input.touchSupported)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initializeLine(Input.GetTouch(0).position);
            }
            else if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary) && currentDist < maxDistance)
            {
                continueLine(Input.GetTouch(0).position);
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endLine();
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0) && !mouseWasDown)
            {
                initializeLine(Input.mousePosition);
            }
            else if(Input.GetMouseButton(0) && currentDist < maxDistance)
            {
                continueLine(Input.mousePosition);
            }

            if((Input.GetMouseButtonUp(0) && mouseWasDown))
            {
                endLine();
            }
        }

        if(startPosition != new Vector3() && currentPosition != new Vector3())
        {
            float dist = Vector3.Distance(startPosition, currentPosition);

            while(startPosition != currentPosition)
            {
                currentDist += Vector3.Distance(startPosition, Vector3.MoveTowards(startPosition, currentPosition, smoothness));
                startPosition = Vector3.MoveTowards(startPosition, currentPosition, smoothness);
                wall.localPosition = startPosition;

                GameObject wallSegment = ((Transform)Instantiate(wall, parent)).gameObject;
                wallSegment.transform.LookAt(Camera.main.transform);

                Vector3 movementOfCollider = Camera.main.transform.position - wallSegment.transform.position;

                float scale = (movementOfCollider.z / 2) * wallSegment.transform.localScale.z;
                movementOfCollider /= scale;

                wallSegment.GetComponent<SphereCollider>().center = new Vector3(0, 0, Vector3.Magnitude(movementOfCollider));

                WallSegmentBehavior behavior = wallSegment.AddComponent<WallSegmentBehavior>();
                behavior.setDuration(lineDuration);
            }
        }
    }
    private void initializeLine(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);

        if (Physics.Raycast(ray, out hit) && hit.transform == background)
        {
            startPosition = hit.point;
            mouseWasDown = true;
            currentPosition = new Vector3();
            currentDist = 0;
        }
    }

    private void continueLine(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);

        if (Physics.Raycast(ray, out hit) && hit.transform == background)
        {
            startPosition = currentPosition;
            currentPosition = hit.point;
            
        }
    }

    private void endLine()
    {
        currentDist = 0;
        mouseWasDown = false;
        startPosition = new Vector3();
    }

    public void powerUP()
    {
        maxDistance = Mathf.Infinity;
        powerupTimer = powerupTime;
    }

    private bool heartPressed(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);

        return (Physics.Raycast(ray, out hit) && hit.transform == player);
    }

    private void explodeHeart()
    {
        impulse.SetActive(true);
        audioController.playImpulseAudio();
        timeHeldDown = 0;
        canDestroyWave = false;

        Destroy(animBucket.GetComponentInChildren<Light>().gameObject);
    }
}
