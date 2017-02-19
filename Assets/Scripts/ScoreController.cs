using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    public GameObject timerText;
    public GameObject scoreText;
    public GameObject WaveText;
    public int initialMultiplier;
    public float multiplierDecayRate;
    public HeartBehavior playerScript;
    public float decayRateIncrease;

    private int scoreThisWave;
    private int totalScore;
    private float timer;
    private int multiplier;
    private int health;

    // Use this for initialization
    void Start()
    {
        scoreThisWave = 0;
        totalScore = 0;
        timer = 0;
        multiplier = initialMultiplier;
        health = playerScript.health;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= multiplierDecayRate)
        {
            timer = 0;
            if(multiplier > 1)
                multiplier--;
        }

        timerText.GetComponent<TextMesh>().text = "" + multiplier;
    }

    public void setWaveText(string text)
    {
        WaveText.GetComponent<TextMesh>().text = text;
    }

    public void updateScore(int points)
    {
        scoreThisWave += points;
        scoreText.GetComponent<TextMesh>().text = "" + (totalScore + scoreThisWave);
    }

    public void updateTotalScore(int currentWave)
    {
        int healthLoss = health - playerScript.health;
        if(healthLoss < 0)
        {
            healthLoss = 0;
        }

        totalScore += (scoreThisWave * multiplier) - healthLoss;
        scoreThisWave = 0;
        scoreText.GetComponent<TextMesh>().text = "" + totalScore;
        multiplier = initialMultiplier;
        multiplierDecayRate += decayRateIncrease;
        timer = 0;
    }

    public int getScore()
    {
        return totalScore;
    }
}
