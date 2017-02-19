using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public GameObject enemyFire;
    public GameObject enemyHit;
    public GameObject enemyDie;
    public GameObject playerHit;
    public GameObject playerDie;
    public AudioSource impulseAudio;

    public void playImpulseAudio()
    {
        impulseAudio.Play();
    }

    public void playEnemyFire()
    {
        playRandomSound(enemyFire);
    }
    
    public void playEnemyHit()
    {
        playRandomSound(enemyHit);
    }

    public void playEnemyDie()
    {

    }

    public void playPlayerHit()
    {
        playRandomSound(playerHit);
    }

    public void playPlayerDie()
    {
        playRandomSound(playerDie);
    }

    private void playRandomSound(GameObject sound)
    {
        ArrayList sounds = new ArrayList(sound.GetComponentsInChildren<AudioSource>());
        int range = (int)(Random.value * sounds.Count);

        AudioSource noise = (AudioSource)sounds[range];
        noise.Play();
    }
}
