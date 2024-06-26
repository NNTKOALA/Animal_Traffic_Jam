using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip touchSound;
    public AudioClip moveSound;
    public AudioClip blockSound;
    public AudioClip escapeSound;

    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlayerSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
