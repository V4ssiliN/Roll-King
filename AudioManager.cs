using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public List<AudioSource> sfxSources;

    [Header("Clips Music")]
    public AudioClip[] musicClips;

    public Dictionary<string, AudioClip> musics = new Dictionary<string, AudioClip>();

    [Header("Clips SFX")]
    public AudioClip[] sfxClips;

    public Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

    public Slider sfxSlider;
    public Slider musicSlider;

    //public AudioClip playerShoot;
    //public AudioClip playerHop;
    //public AudioClip pistoleroShoot;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Il y a plus d'une instance de PlayerMovement dans la scène !");
            return;
        }
        instance = this;

        foreach (AudioClip music in musicClips)
        {
            musics[music.name] = music;
        }

        foreach (AudioClip clip in sfxClips)
        {
            soundEffects[clip.name] = clip;
        }
    }

    public void PlayMusic(string clipName)
    {
        if (musics.TryGetValue(clipName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySfx(string clipName)
    {
        if (soundEffects.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSources[0].PlayOneShot(clip);
        }
    }

    public void ChangeMusicVolume(float volume = -1f)
    {
        if (volume < 0)
        {
            musicSource.volume = musicSlider.value; 
        }
        else
        {
            musicSource.volume = volume;
        }
    }

    public void ChangeSFXVolume(float volume = -1f)
    {
        foreach (AudioSource source in sfxSources)
        {
            if (volume < 0)
            {
                source.volume = sfxSlider.value; 
            }
            else
            {
                source.volume = volume;
            }
        }
    }

    public void SetSliders()
    {
        musicSlider.value = musicSource.volume;
        sfxSlider.value = sfxSources[0].volume;
    }
}
