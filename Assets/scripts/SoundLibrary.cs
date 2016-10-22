using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;
    public MusicGroup[] musicGroups;

    Dictionary<string, AudioClip[]> soundClips = new Dictionary<string, AudioClip[]>();

    Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    void Awake()
    {
        // Load Sounds into SoundClip Dictionary 
        foreach (SoundGroup soundGroup in soundGroups)
        {
            soundClips.Add(soundGroup.soundID, soundGroup.group);
        }

        // Load Music into MusicClip Dictionary
        foreach (MusicGroup musicGroup in musicGroups)
        {
            musicClips.Add(musicGroup.musicID, musicGroup.music);
        }
    }

    public AudioClip GetMusicFromName(string name)
    {
        if (musicClips.ContainsKey(name))
        {
            AudioClip music = musicClips[name];
            return music;
        }
        else
        {
            Debug.LogError("Music Clip not found : " + name);
            return null;
        }
    }

    public AudioClip GetClipFromName(string name)
    {
        if (soundClips.ContainsKey(name))
        {
            AudioClip[] sounds = soundClips[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        else
        {
            Debug.LogError("Sound Clip not found : " + name);
            return null;
        }
    }

    [System.Serializable]
    public class SoundGroup
    {
        public string soundID;
        public AudioClip[] group;
    }

    [System.Serializable]
    public class MusicGroup
    {
        public string musicID;
        public AudioClip music;
    }
}