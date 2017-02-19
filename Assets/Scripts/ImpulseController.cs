using UnityEngine;
using System.Collections;

public class ImpulseController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "bullet" || other.tag == "powerup")
        {
            Destroy(other.gameObject);
        }
        else if(other.tag == "enemy" || other.tag == "spikeball")
        {
            other.transform.parent.GetComponent<EnemyBehavior>().doCollision(other.tag);
        }
    }
}
