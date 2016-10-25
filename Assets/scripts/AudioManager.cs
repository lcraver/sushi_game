using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SoundLibrary))]

public class AudioManager : MonoBehaviour
{

    string className = "[<color=#2196F3>AudioManager</color>] \n";

    public string currentSongName = "";

    public float masterVol = 1;
    public float musicVol = 1;
    public float sfxVol = 1;

    public float pitch;

    public bool isFading = false;
    public AudioSource[] musicSources;
    public int activeMusicSourceIndex;

    public List<AudioSource> soundSources = new List<AudioSource>();

    SoundLibrary library;
    [HideInInspector]
    public static AudioManager inst;

    void Awake()
    {
        if (!inst)
        {
            inst = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        musicSources = new AudioSource[2];
        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i] == null)
            {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.GetComponent<AudioSource>().volume = 0;
                newMusicSource.GetComponent<AudioSource>().loop = true;
                newMusicSource.transform.parent = transform;
            }
        }

        library = GetComponent<SoundLibrary>();
    }

    /// <summary>
    /// Updates the volumes.
    /// </summary>
    void Update()
    {
        if (!isFading)
            musicSources[activeMusicSourceIndex].volume = musicVol;

        musicSources[activeMusicSourceIndex].pitch = pitch;
    }

    public bool IsPlaying()
    {
        return musicSources[activeMusicSourceIndex].isPlaying;
    }

    /// <summary>
    /// Sets the music time to the time specified.
    /// </summary>
    /// <param name="time">The time in seconds to set the music time to.</param>
    public void SetMusicTime(float time)
    {
        if (musicSources[activeMusicSourceIndex].clip != null)
        {
            if (musicSources[activeMusicSourceIndex].clip.length >= time && time >= 0)
            {
                Debug.LogFormat("{0}Setting music time to [{1}]", className, time);
                musicSources[activeMusicSourceIndex].time = time;
            }
            else
            {
                Debug.LogErrorFormat("{0}Music time is out of bounds [{1}]", className, time);
            }
        }
    }

    public void PlayMusic(AudioClip clip, bool same = false, float fadeDuration = 1)
    {
        if (clip != null)
        {
            if (clip.name != currentSongName || same)
            {
                Debug.LogFormat("{0}Started playing music clip [{1}]", className, clip.name);
                currentSongName = clip.name;
                activeMusicSourceIndex = 1 - activeMusicSourceIndex;
                CurrentAudioSource.clip = clip;
                CurrentAudioSource.UnPause();
                CurrentAudioSource.time = 0;
                CurrentAudioSource.Play();
                StartCoroutine(animateMusicFade(fadeDuration));
            }
        }
        else
        {
            Debug.LogErrorFormat("{0}Couldn't play music clip [{1}]", className, clip.name);
        }
    }

    /// <summary>
    /// Stop playing all the music.
    /// </summary>
    public void StopMusic()
    {
        Debug.LogFormat("{0}Stopped playing music", className);
        for (int i = 0; i < musicSources.Length; i++)
        {
            musicSources[i].GetComponent<AudioSource>().Pause();
        }
    }

    /// <summary>
    /// Plays a given audioclip at the given locaiton.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="pos">The position to play the clip at.</param>
    public void PlaySoundClip(AudioClip _clip, float _vol = 1f)
    {
        if (_clip != null)
        {
            Debug.LogFormat("{0}Played sound [{1}]", className, _clip.name);

            AudioSource newSound = Camera.main.gameObject.AddComponent<AudioSource>();
            newSound.clip = _clip;
            newSound.playOnAwake = true;
            newSound.loop = false;
            newSound.volume = _vol;
            newSound.Play();

            StartCoroutine(DestroyWithDelay(newSound, _clip.length));
        }
        else
        {
            Debug.LogErrorFormat("{0}Couldn't play sound", className);
        }
    }

    public IEnumerator DestroyWithDelay(AudioSource _audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(_audioSource);
    }

    /// <summary>
    /// Plays a given sound at the given locaiton.
    /// </summary>
    /// <param name="soundName">The name of the sound.</param>
    public void PlaySound(string soundName, float _vol = 1f)
    {
        PlaySoundClip(library.GetClipFromName(soundName), _vol);
    }

    /// <summary>
    /// Plays a given sound at the given locaiton.
    /// </summary>
    /// <param name="musicName">The name of the sound.</param>
    /// <param name="pos">The position to play the clip at.</param>
    public void PlayMusic(string musicName, float fade)
    {
        PlayMusic(library.GetMusicFromName(musicName), false, fade);
    }


    public AudioClip GetSound(string soundName)
    {
        return library.GetClipFromName(soundName);
    }

    public AudioClip GetMusic(string soundName)
    {
        return library.GetMusicFromName(soundName);
    }

    public void SetPitch(float _pitch)
    {
        Debug.Log("Set Pitch : " + _pitch);
        pitch = _pitch;
    }

    IEnumerator animateMusicFade(float duration)
    {
        isFading = true;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVol, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVol, 0, percent);
            yield return null;
        }

        isFading = false;
    }

    public AudioSource CurrentAudioSource
    {
        get { return musicSources[activeMusicSourceIndex]; }
    }
}
