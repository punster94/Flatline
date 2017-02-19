using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeartBehavior : MonoBehaviour
{
    public int health;
    public float maxTimeBetweenPulses;
    public string pulseTriggerName;
    public GameObject audio;
    public ScoreController scoreController;
    public ScoreStats scoreStats;
    public SpawnController spawnController;
    public InputController inputController;
    public GameObject animBucket;

    public GameObject[] heartPhases;

    private Animator animator;

    private float timeBetweenPulses;
    private float timeOfLastPulse;
    private int startHealth;

    void Start()
    {
        health = 100;
        maxTimeBetweenPulses = 5;

        animator = gameObject.GetComponentInChildren<Animator>();

        timeBetweenPulses = maxTimeBetweenPulses;
        timeOfLastPulse = Time.time;

        startHealth = health;
    }

    void Update()
    {
        if (Time.time - timeOfLastPulse >= timeBetweenPulses)
        {
            animator.SetTrigger(pulseTriggerName);
            timeOfLastPulse = Time.time;
        }
    }

    public void doCollision(int damage, BulletType type, Vector3 position, Vector3 velocity)
    {
        switch (type)
        {
            case BulletType.Bullet:
                bulletCollision(damage, position, velocity);
                break;
            case BulletType.Health:
                playPickupAnim();
                healthCollision(damage);
                break;
            case BulletType.Pill:
                playPickupAnim();
                pillCollision();
                break;
            case BulletType.Star:
                playPickupAnim();
                starCollision();
                break;
        }
    }

    private void playPickupAnim()
    {
        GameObject anim = Instantiate(Resources.Load("PickupPowerup", typeof(GameObject))) as GameObject;
        anim.transform.parent = animBucket.transform;
        anim.transform.position = gameObject.transform.parent.position;
    }

    private void pillCollision()
    {
        spawnController.startSlowdown();
    }

    private void healthCollision(int damage)
    {
        health += damage;
        health = (int) Mathf.Min(health, 100f);
        modifyHealthBar();
    }

    private void bulletCollision(int damage, Vector3 position, Vector3 velocity)
    {
        health -= damage;
        modifyHealthBar();

        GameObject hitAnimation = Instantiate(Resources.Load("HitAnimation", typeof(GameObject))) as GameObject;
        hitAnimation.transform.parent = animBucket.transform;
        hitAnimation.transform.position = position;

        Vector3 rotation = new Vector3(0, 0, Vector3.Angle(Vector3.down, velocity));

        if (position.x < transform.GetChild(0).position.x)
            hitAnimation.transform.Rotate(rotation);
        else
            hitAnimation.transform.Rotate(-rotation);


        if (health <= 66 && health > 33 && !inputController.gotFirstExplosion)
        {
            inputController.gotFirstExplosion = true;
            inputController.canDestroyWave = true;

            GameObject readyAnimation = Instantiate(Resources.Load("ReadyLight", typeof(GameObject))) as GameObject;
            readyAnimation.transform.parent = animBucket.transform;
            readyAnimation.transform.position = transform.position;
        }
        else if (health <= 33 && !inputController.gotSecondExplosion && !inputController.canDestroyWave)
        {
            inputController.gotSecondExplosion = true;
            inputController.canDestroyWave = true;

            GameObject readyAnimation = Instantiate(Resources.Load("ReadyLight", typeof(GameObject))) as GameObject;
            readyAnimation.transform.parent = animBucket.transform;
            readyAnimation.transform.position = transform.position;
        }

        if (health <= 0)
        {
            scoreController.updateTotalScore(0);
            scoreStats.score = scoreController.getScore();
            //scoreStats.timeLeft = scoreController.timer;

            audio.GetComponent<AudioController>().playPlayerDie();
            SceneManager.LoadSceneAsync("ScoreScreen");
        }
        else
        {
            audio.GetComponent<AudioController>().playPlayerHit();
            timeBetweenPulses = ((float)health / startHealth) * maxTimeBetweenPulses;
        }

    }

    private void starCollision()
    {
        inputController.powerUP();
    }

    private void modifyHealthBar()
    {
        foreach(GameObject g in heartPhases)
        {
            g.SetActive(false);
        }

        heartPhases[(int)((heartPhases.Length - 1) * (health + 10) / 100)].SetActive(true);
    }
}
