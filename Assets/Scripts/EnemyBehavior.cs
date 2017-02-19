using UnityEngine;
using System.Collections;

public enum BulletType { Bullet, Health, Pill, Star }
public class EnemyBehavior : MonoBehaviour
{
    public GameObject player;
    public GameObject bulletBucket;
    public GameObject animBucket;
    public GameObject UI;
    public GameObject audio;

    public float fireRate;
    public float timeUntilSpawn;

    private int movementPaternIndex;
    private int slowMovementPaternIndex;
    private int rotationPaternIndex;
    private int slowRotationPaternIndex;

    private static Vector3[] movementPatern;
    private static Vector3[] rotationPatern;

    private static Vector3[] slowMovementPatern;
    private static Vector3[] slowRotationPatern;

    public float bulletSize = 1;
    public int bulletDamage;

    private float bulletSpeed = 4f;
    private float timeSinceLastBullet;

    private float startTime;

    public bool spawned;
    private int numBullets = 16;
    private bool doExplosion;
    private bool slowed;

	// Use this for initialization
	void Start ()
    {
        doExplosion = false;
        slowed = false;

        movementPaternIndex = 0;
        timeSinceLastBullet = 0;

        spawned = false;
        startTime = Time.time;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (doExplosion)
        {
            explode();
        }

        if (Time.time - startTime >= timeUntilSpawn)
        {
            spawned = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (spawned)
        {
            timeSinceLastBullet += Time.deltaTime;

            if (timeSinceLastBullet >= fireRate)
            {
                createProjectile(BulletType.Bullet);
                audio.GetComponent<AudioController>().playEnemyFire();
                timeSinceLastBullet = 0;
            }

            transform.position += nextMovementInPatern();
            transform.eulerAngles += nextRotationInPatern();
        }
    }

    public BulletBehavior createProjectile(BulletType type)
    {
        GameObject bullet = null;

        switch(type)
        {
            case BulletType.Bullet:
                bullet = Instantiate(Resources.Load("BulletFr_01", typeof(GameObject))) as GameObject;
                bullet.transform.localScale *= bulletSize;
                break;
            case BulletType.Health:
                bullet = Instantiate(Resources.Load("HealthPU", typeof(GameObject))) as GameObject;
                break;
            case BulletType.Pill:
                bullet = Instantiate(Resources.Load("SlowDownPU", typeof(GameObject))) as GameObject;
                break;
            case BulletType.Star:
                bullet = Instantiate(Resources.Load("StarPU", typeof(GameObject))) as GameObject;
                break;
        }

        bullet.AddComponent<BulletBehavior>().initialize(transform.position, player, bulletSpeed, audio, UI, type, slowed, bulletDamage);

        bullet.transform.parent = bulletBucket.transform;

        return bullet.GetComponent<BulletBehavior>();
    }

    public void slowDown()
    {
        slowed = true;
        slowMovementPaternIndex = movementPaternIndex * 2;
        slowRotationPaternIndex = rotationPaternIndex * 2;
    }

    public void restoreSpeed()
    {
        slowed = false;

        // preserve the circle they are on

        if (slowMovementPaternIndex % 2 == 1)
        {
            transform.position += nextMovementInPatern();
            transform.eulerAngles += nextRotationInPatern();
        }

        movementPaternIndex = slowMovementPaternIndex / 2;
        rotationPaternIndex = slowRotationPaternIndex / 2;
    }

    private Vector3 nextMovementInPatern()
    {
        if (movementPatern == null)
            createMovementPatern();

        Vector3 nextMovement;

        if (slowed)
        {
            nextMovement = slowMovementPatern[slowMovementPaternIndex];
            slowMovementPaternIndex++;
            slowMovementPaternIndex %= slowMovementPatern.Length;
        }
        else
        {
            nextMovement = movementPatern[movementPaternIndex];
            movementPaternIndex++;
            movementPaternIndex %= movementPatern.Length;
        }

        return nextMovement;
    }

    private Vector3 nextRotationInPatern()
    {
        if (rotationPatern == null)
            createRotationPatern();

        Vector3 nextRotation;

        if (slowed)
        {
            nextRotation = slowRotationPatern[slowRotationPaternIndex];
            slowRotationPaternIndex++;
            slowRotationPaternIndex %= slowRotationPatern.Length;
        }
        else
        {
            nextRotation = rotationPatern[rotationPaternIndex];
            rotationPaternIndex++;
            rotationPaternIndex %= rotationPatern.Length;
        }

        return nextRotation;
    }

    protected void createMovementPatern()
    {
        int paternSize = 100;
        float distancePerIndex = 0.1f;

        movementPatern = new Vector3[paternSize];
        slowMovementPatern = new Vector3[paternSize * 2];

        for(int i = 0; i < paternSize; i++)
        {
            movementPatern[i] = new Vector3(Mathf.Cos(Mathf.PI * 2 * i / paternSize), Mathf.Sin(Mathf.PI * 2 * i / paternSize));
            movementPatern[i] *= distancePerIndex;

            slowMovementPatern[2 * i] = new Vector3(Mathf.Cos(Mathf.PI * 2 * i / paternSize), Mathf.Sin(Mathf.PI * 2 * i / paternSize));
            slowMovementPatern[2 * i] *= (distancePerIndex / 2);

            slowMovementPatern[2 * i + 1] = new Vector3(Mathf.Cos(Mathf.PI * 2 * i / paternSize), Mathf.Sin(Mathf.PI * 2 * i / paternSize));
            slowMovementPatern[2 * i + 1] *= (distancePerIndex / 2);
        }
    }

    protected void createRotationPatern()
    {
        int paternSize = 100;
        float anglePerIndex = 0.05f;

        rotationPatern = new Vector3[paternSize];
        slowRotationPatern = new Vector3[paternSize * 2];

        for(int i = 0; i < paternSize; i++)
        {
            rotationPatern[i] = new Vector3(Mathf.Rad2Deg * Mathf.Cos(Mathf.PI * 2 * i / paternSize), Mathf.Rad2Deg * Mathf.Sin(Mathf.PI * 2 * i / paternSize));
            rotationPatern[i] *= anglePerIndex;

            slowRotationPatern[2 * i] = new Vector3(Mathf.Rad2Deg * Mathf.Cos(Mathf.PI * 2 * i / paternSize), Mathf.Rad2Deg * Mathf.Sin(Mathf.PI * 2 * i / paternSize));
            slowRotationPatern[2 * i] *= (anglePerIndex / 2);

            slowRotationPatern[2 * i + 1] = new Vector3(Mathf.Rad2Deg * Mathf.Cos(Mathf.PI * 2 * i / paternSize), Mathf.Rad2Deg * Mathf.Sin(Mathf.PI * 2 * i / paternSize));
            slowRotationPatern[2 * i + 1] *= (anglePerIndex / 2);
        }
    }

    public void doCollision(string tag)
    {
        UI.GetComponent<ScoreController>().updateScore(10);
        audio.GetComponent<AudioController>().playEnemyHit();

        if (tag == "spikeball")
        {
            doExplosion = true;
        }
        else
        {
            GameObject explosion = getRandomExplosion();
            Destroy(gameObject);
        }
    }

    public void explode()
    {
        GameObject explosion = getRandomExplosion();
        
        int randomDirection = 2;

        while(randomDirection == 2)
            randomDirection = (int)(Random.value * 5);

        randomDirection -= 2;
        randomDirection += numBullets;
        randomDirection %= numBullets;

        for (int i = 0; i < numBullets; i++)
        {
            if (i == randomDirection)
                createProjectile(BulletType.Health).changeInitialState(transform.position, i * (360f / numBullets));
            else
                createProjectile(BulletType.Bullet).changeInitialState(transform.position, i * (360f / numBullets));
        }

        Destroy(gameObject);
    }

    private GameObject getRandomExplosion()
    {
        GameObject explosion;

        switch((int) (Random.value * 3))
        {
            case 0:
                explosion = Instantiate(Resources.Load("ExplosionAnim", typeof(GameObject))) as GameObject;
                break;
            case 1:
                explosion = Instantiate(Resources.Load("ExplosionAnim2", typeof(GameObject))) as GameObject;
                break;
            case 2:
                explosion = Instantiate(Resources.Load("Explosion3", typeof(GameObject))) as GameObject;
                break;
            default:
                explosion = Instantiate(Resources.Load("ExplosionAnim", typeof(GameObject))) as GameObject;
                break;
        }

        explosion.transform.position = gameObject.transform.position;
        explosion.transform.parent = animBucket.transform;

        return explosion;
    }
}
