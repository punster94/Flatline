using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreScreenController : MonoBehaviour
{
    private float timeHeldDown;
    private ScoreStats scoreStats;
    private AsyncOperation op;

    public Transform player;
    public float timeToStart;

    public TextMesh scoreText;
    public TextMesh timeText;

    void Start()
    {
        timeHeldDown = 0;
        scoreStats = GameObject.Find("ScoreStats").GetComponent<ScoreStats>();

        scoreText.text = "Score: " + scoreStats.score;
        //timeText.text = "Time Remaining: " + scoreStats.timeLeft;

        // Remove this after used to a duplicate item is not made when the main scene is loaded again
        Destroy(scoreStats.gameObject);

        op = SceneManager.LoadSceneAsync("LoadingScreen");
        op.allowSceneActivation = false;
    }

    void Update()
    {
        if (timeHeldDown >= timeToStart)
        {
            op.allowSceneActivation = true;
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
