using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    private float timeHeldDown;
    private bool startGame;
    private AsyncOperation op;

    public Transform player;
    public float holdTime;
    public GameObject impulse;
    public float waitTimer;

    void Start()
    {
        timeHeldDown = 0;
        startGame = false;

        op = SceneManager.LoadSceneAsync("LoadingScreen");
        op.allowSceneActivation = false;
    }

    void Update()
    {
        if(timeHeldDown >= holdTime)
        {
            impulse.SetActive(true);
            startGame = true;
        }

        if(startGame)
        {
            waitTimer -= Time.deltaTime;
            if(waitTimer <= 0)
            {
                op.allowSceneActivation = true;
            }
        }

        if (Input.touchSupported)
        {
            if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                if (Physics.Raycast(ray, out hit) && hit.transform == player)
                {
                    timeHeldDown += Time.deltaTime;
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                timeHeldDown = 0;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && hit.transform == player)
                {
                    timeHeldDown += Time.deltaTime;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                
                timeHeldDown = 0;
            }
        }
    }
}
