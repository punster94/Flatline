using UnityEngine;
using System.Collections;

public class BulletBehavior : MonoBehaviour
{
    private float bulletStartDistance = 1.5f;
    private float speed;

    private Rigidbody bulletRigidbody;
    private GameObject audio;
    private GameObject UI;
    private GameObject player;
    private bool hasBounced;
    private Vector3 lastVelocity;
    private bool slowed;

    public int damagePercent;
    public BulletType type;
    
    void Start()
    {

    }

    public void initialize(Vector3 ownerPosition, GameObject p, float s, GameObject a, GameObject ui, BulletType t, bool slow, int bulletDamage)
    {
        speed = s;
        audio = a;
        UI = ui;
        player = p;
        type = t;
        damagePercent = bulletDamage;

        switch (type)
        {
            case BulletType.Health:
                damagePercent = 20;
                break;
            case BulletType.Pill:
                damagePercent = 0;
                break;
            default:
                break;
        }

        // calculate initial direction of bullet given owners position and position of player
        Vector3 direction = Vector3.Normalize(p.transform.position - ownerPosition);

        // initialize the bullet location
        gameObject.transform.position = ownerPosition + direction * bulletStartDistance;

        bulletRigidbody = gameObject.GetComponent<Rigidbody>();
        bulletRigidbody.useGravity = false;
        bulletRigidbody.isKinematic = false;
        bulletRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        bulletRigidbody.velocity = direction * speed;
        lastVelocity = bulletRigidbody.velocity;

        slowed = slow;
        if (slow)
            slowDown();
    }

    public void changeInitialState(Vector3 ownerPosition, float angle)
    {
        float currentAngle = Vector3.Angle(Vector3.forward, bulletRigidbody.velocity);

        if (player.transform.position.x < transform.position.x)
            currentAngle += 180;

        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle + currentAngle)), Mathf.Cos(Mathf.Deg2Rad * (angle + currentAngle)));
        gameObject.transform.position = ownerPosition + direction * bulletStartDistance * 1.5f;
        bulletRigidbody.velocity = direction * speed;
        lastVelocity = bulletRigidbody.velocity;
    }

    public void slowDown()
    {
        slowed = true;
        speed /= 2;
        bulletRigidbody.velocity /= 2;
        lastVelocity = bulletRigidbody.velocity;
    }

    public void restoreSpeed()
    {
        slowed = false;
        speed *= 2;
        bulletRigidbody.velocity *= 2;
        lastVelocity = bulletRigidbody.velocity;
    }

    void Update()
    {
        // destroy bullet if outside of bounds of camera
        if (transform.position.x > 16 || transform.position.x < -16 || transform.position.y > 10 || transform.position.y < -10)
            GameObject.Destroy(gameObject);

        if(bulletRigidbody.velocity.magnitude != speed)
        {
            bulletRigidbody.velocity = lastVelocity;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(hasBounced && (other.tag == "enemy" || other.tag == "spikeball") && type == BulletType.Bullet)
        {
            other.transform.parent.gameObject.GetComponent<EnemyBehavior>().doCollision(other.tag);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = null;
        if (collision.contacts.Length > 0)
            collidedObject = collision.contacts[0].otherCollider.gameObject;

        if (collidedObject != null)
        {
            
            switch (collidedObject.tag)
            {
                case "player":
                    collidedObject.GetComponent<HeartBehavior>().doCollision(damagePercent, type, transform.position, lastVelocity);
                    Destroy(gameObject);
                    break;
                case "wall":
                    hasBounced = true;
                    setBulletVelocity(collision);
                    break;
                default:
                    setBulletVelocity(collision);
                    break;
            }
        }
    }

    private void setBulletVelocity(Collision collision)
    {
        Vector3 normal = new Vector3();
        foreach (ContactPoint c in collision.contacts)
        {
            normal += c.normal;
        }

        normal = new Vector3(normal.x, normal.y).normalized;
        if (normal == Vector3.zero)
        {
            bulletRigidbody.velocity = lastVelocity;
        }
        else
        {
            bulletRigidbody.velocity = normal * speed;
            lastVelocity = bulletRigidbody.velocity;
        }
    }
}
