using UnityEngine;
using System.Collections;

public class ScoreStats : MonoBehaviour
{
    public float timeLeft;
    public int score;

	void Start ()
    {
        DontDestroyOnLoad(gameObject);
    }
}
