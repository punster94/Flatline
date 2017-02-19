using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SpawnController : MonoBehaviour
{
    public GameObject enemyBucket;
    public GameObject bulletBucket;
    public Transform playerPosition;
    public Transform bulletTarget;
    public GameObject UI;
    public GameObject audio;
    public ScoreStats scoreStats;
    public GameObject animBucket;
    public GameObject impulse;

    public GameObject jaxEnemy;
    public GameObject tieEnemy;
    public GameObject spikeBall;

    public int numberOfWaves;
    public int minEnemiesInWave;
    public int maxEnemiesInWave;

    public float maxSpawnTime;
    public float minSpawnTime;

    public int numberOfEnemyTypes;
    public float timeBetweenPowerups;
    public int numberOfPowerups;
    public float sliceSize;
    public float slowDuration;

    private int currentWave;
    private GameObject[][] waves;
    private Vector3[] spawnLocations;

    private int numberOfSpawnLocations = 8;
    private float timeOfLastPowerup;

    private bool slowed;
    private float timeSinceSlowed;

	// Use this for initialization
	void Start ()
    {
        setSpawnLocations();
        createWaves();
        currentWave = 0;
        timeOfLastPowerup = 0;
        slowed = false;
        moveToNextWave();
	}
	
	// Update is called once per frame
	void Update ()
    {
        int currentNumberOfChildren = 0;
        int numSpikeballs = 0;

        foreach(Transform child in enemyBucket.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                currentNumberOfChildren++;

                if(child.gameObject.tag == "spikeball")
                {
                    numSpikeballs++;
                }
            }
        }

        if(currentNumberOfChildren == numSpikeballs && bulletBucket.transform.childCount == 0)
        {
            foreach (Transform child in enemyBucket.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    child.GetComponent<EnemyBehavior>().explode();
                }
            }

            currentNumberOfChildren = 0;
        }

	    if (currentNumberOfChildren == 0 && bulletBucket.transform.childCount == 0)
        {
            impulse.SetActive(false);
            if (Input.touchSupported)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && heartTouched(Input.GetTouch(0).position))
                {
                    moveToNextWave();
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) && heartTouched(Input.mousePosition))
                { 
                    moveToNextWave();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || (Time.time >= timeOfLastPowerup + timeBetweenPowerups && (currentNumberOfChildren > 0 && currentNumberOfChildren != numSpikeballs)))
            {
                BulletType type;

                switch((int)(Random.value * numberOfPowerups))
                {
                    case 0:
                        type = BulletType.Health;
                        break;
                    case 1:
                        type = BulletType.Pill;
                        break;
                    case 2:
                        type = BulletType.Star;
                        break;
                    default:
                        type = BulletType.Health;
                        break;
                }

                // picks a position within the slice
                float randomAngle = Random.value * sliceSize;
                randomAngle -= sliceSize / 2;

                // picks an enemy to perform the powerup creation
                if (enemyBucket.transform.childCount > 0)
                {
                    BulletBehavior powerup = enemyBucket.transform.GetChild((int)(Random.value * (enemyBucket.transform.childCount - 1))).GetComponent<EnemyBehavior>().createProjectile(type);
                    powerup.changeInitialState(enemyBucket.transform.GetChild(0).transform.position, randomAngle);

                    timeOfLastPowerup = Time.time;
                }
            }
        }

        if (slowed && (Time.time >= timeSinceSlowed + slowDuration))
        {
            slowed = false;
            restoreAllSpeed();
        }
	}

    public void startSlowdown()
    {
        slowed = true;
        slowEverything();
        timeSinceSlowed = Time.time;
    }

    private void slowEverything()
    {
        foreach(Transform child in bulletBucket.transform)
            child.GetComponent<BulletBehavior>().slowDown();

        foreach (Transform child in enemyBucket.transform)
            child.GetComponent<EnemyBehavior>().slowDown();
    }

    private void restoreAllSpeed()
    {
        foreach(Transform child in bulletBucket.transform)
            child.GetComponent<BulletBehavior>().restoreSpeed();

        foreach (Transform child in enemyBucket.transform)
            child.GetComponent<EnemyBehavior>().restoreSpeed();
    }

    private bool heartTouched(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);

        return Physics.Raycast(ray, out hit) && hit.transform == playerPosition;
    }

    private void moveToNextWave()
    {
        foreach (Transform t in animBucket.transform)
        {
            Destroy(t.gameObject);
        }

        timeOfLastPowerup = Time.time;
        if (currentWave == numberOfWaves)
        {
            UI.GetComponent<ScoreController>().updateTotalScore(currentWave);
            scoreStats.score = UI.GetComponent<ScoreController>().getScore();
            //scoreStats.timeLeft = UI.GetComponent<ScoreController>().timer;

            SceneManager.LoadSceneAsync("ScoreScreen");
        }
        else
        {
            foreach (GameObject enemy in waves[currentWave])
            {
                enemy.SetActive(true);
            }

            currentWave++;
            UI.GetComponent<ScoreController>().setWaveText(""+currentWave);
            UI.GetComponent<ScoreController>().updateTotalScore(currentWave);
        }
    }

    private void createWaves()
    {
        waves = new GameObject[numberOfWaves][];

        for (int wave = 0; wave < numberOfWaves; wave++)
        {
            int numberOfEnemies = wave + minEnemiesInWave;
            waves[wave] = new GameObject[numberOfEnemies];

            ArrayList locationsPicked = new ArrayList();

            int numSpikeBalls = 0;

            for (int enemy = 0; enemy < numberOfEnemies; enemy++)
            {
                GameObject newEnemy = new GameObject();
                waves[wave][enemy] = newEnemy;

                newEnemy.transform.parent = enemyBucket.transform;

                EnemyBehavior enemyBehavior = newEnemy.AddComponent<EnemyBehavior>();
                enemyBehavior.player = bulletTarget.gameObject;
                enemyBehavior.bulletBucket = bulletBucket;
                //enemyBehavior.bullet = bullet;
                enemyBehavior.UI = UI;
                enemyBehavior.audio = audio;
                enemyBehavior.animBucket = animBucket;

                Rigidbody body = newEnemy.AddComponent<Rigidbody>();
                body.useGravity = false;
                body.isKinematic = true;

                GameObject newEnemyMesh = null;

                switch((int)(Random.value * 10))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        enemyBehavior.fireRate = 3f;
                        enemyBehavior.bulletSize = 2f;
                        enemyBehavior.bulletDamage = 10;
                        newEnemyMesh = Instantiate(jaxEnemy);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        enemyBehavior.fireRate = 1f;
                        enemyBehavior.bulletSize = 1f;
                        enemyBehavior.bulletDamage = 5;
                        newEnemyMesh = Instantiate(tieEnemy);
                        break;
                    case 8:
                    case 9:
                        numSpikeBalls++;
                        enemyBehavior.fireRate = Mathf.Infinity;
                        newEnemyMesh = Instantiate(spikeBall);
                        newEnemy.tag = "spikeball";
                        break;
                    default:
                        enemyBehavior.fireRate = 3f;
                        enemyBehavior.bulletSize = 2f;
                        enemyBehavior.bulletDamage = 10;
                        newEnemyMesh = Instantiate(jaxEnemy);
                        break;
                }
                newEnemyMesh.transform.parent = newEnemy.transform;

                enemyBehavior.timeUntilSpawn = minSpawnTime * (enemy+1);

                Vector3 randomPosition = spawnLocations[(int)(Random.value * (numberOfSpawnLocations - 1))];

                while(locationsPicked.Contains(randomPosition))
                {
                    randomPosition = spawnLocations[(int)(Random.value * (numberOfSpawnLocations - 1))];
                }

                newEnemy.transform.localPosition = randomPosition;
                locationsPicked.Add(randomPosition);

                newEnemy.SetActive(false);
            }
        }
    }

    private void setSpawnLocations()
    {
        spawnLocations = new Vector3[numberOfSpawnLocations];

        spawnLocations[0] = new Vector3(10, 5.5f);
        spawnLocations[1] = new Vector3(-12, 5f);
        spawnLocations[2] = new Vector3(12, 1.5f);
        spawnLocations[3] = new Vector3(6.5f, -6f);
        spawnLocations[4] = new Vector3(11, -4.5f);
        spawnLocations[5] = new Vector3(-6.5f, -5);
        spawnLocations[6] = new Vector3(-12.5f, -4.5f);
        spawnLocations[7] = new Vector3(-8.5f, 0);
    }
}
