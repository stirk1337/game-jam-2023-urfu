using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpRuneManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject expRunePrefab;
    [SerializeField] AudioSource levelUpSound;
    [SerializeField] AudioSource collectAudio;

    public void Spawn(Transform target)
    {
        Instantiate(expRunePrefab, target.position, transform.rotation);
    }

    public void SoundLevelUp()
    {
        levelUpSound.Play();
    }

    public void CollectSound()
    {
        collectAudio.Play();
    }
}
