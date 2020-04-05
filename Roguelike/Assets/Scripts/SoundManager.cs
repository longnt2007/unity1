using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSouce;
    public AudioSource musicSouce;
    public static SoundManager instance = null;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle (AudioClip clip)
    {
        efxSouce.clip = clip;
        efxSouce.Play();
    }

    public void RandomizeSfx( params AudioClip [] clips)
    {
        int randomIndex = Random.Range (0, clips.Length);
        float randomPitch = Random.Range (lowPitchRange, highPitchRange);

        efxSouce.pitch = randomPitch;
        efxSouce.clip = clips[randomIndex];
        efxSouce.Play();
    }
}
