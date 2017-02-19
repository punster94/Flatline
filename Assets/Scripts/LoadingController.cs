using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public float timer;
    public GameObject EKGPulse;
    public GameObject EKGFlat;

    private AsyncOperation op;

    // Use this for initialization
    void Start()
    {
        op = SceneManager.LoadSceneAsync("MainScene");
        op.allowSceneActivation = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 2f)
        {
            EKGPulse.SetActive(false);
            EKGFlat.SetActive(true);
        }
        if(timer <= 0)
        {
            op.allowSceneActivation = true;
        }
    }
}
