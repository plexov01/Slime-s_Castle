using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_AudioManager: MonoBehaviour
{
    public static scr_AudioManager instance = null;

    public static AudioSource musicAudioSource;

    [Header("Sounds")]
    [SerializeField] private SoundAudioClip[] soundAudioClipArray;

    [Header("Music")]
    [SerializeField] private MusicAudioClip[] musicAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public string sound;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class MusicAudioClip
    {
        public string music;
        public AudioClip audioClip;
    }

    private void Awake()
    {
        musicAudioSource = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic("Default");
    }

    public void UpdateVolume()
    {
        if (musicAudioSource == null)
        {
            musicAudioSource = GetComponent<AudioSource>();
        }

        musicAudioSource.volume = MenuController.soundVolume;
    }

    public static void PlaySound(string sound, GameObject gameObject)
    {
        string name = sound.ToString() + "Sound";
        AudioClip audioClip = GetSoundAudioClip(sound);

        if (audioClip != null)
        {
            GameObject soundGameObject = new GameObject(name);
            soundGameObject.transform.SetParent(gameObject.transform);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(audioClip, MenuController.soundVolume);

            if (soundGameObject != null)
            {
                Destroy(soundGameObject, audioClip.length);
            }
        }
    }

    public static void PlaySoundAtPosition(string sound, Vector3 pos)
    {
        string name = sound.ToString() + "Sound";
        AudioClip audioClip = GetSoundAudioClip(sound);

        if (audioClip != null)
        {
            GameObject soundGameObject = new GameObject(name);
            soundGameObject.transform.position = pos;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(audioClip, MenuController.soundVolume);
            
            if (soundGameObject != null)
            {
                Destroy(soundGameObject, audioClip.length);
            }
        }
    }

    public static void PlayRepeatSound(string sound, GameObject gameObject, float interval)
    {
        string name = sound.ToString() + "Sound";
        AudioClip audioClip = GetSoundAudioClip(sound);

        if (audioClip != null)
        {
            GameObject soundGameObject = new GameObject(name);
            soundGameObject.transform.SetParent(gameObject.transform);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

            float cooldown = audioClip.length + interval;
            instance.StartCoroutine(Repeat(cooldown, audioSource, audioClip, soundGameObject, 0));
        }
    }

    public static void PlayRepeatSound(string sound, GameObject gameObject, float interval, int numberOfTimes)
    {
        string name = sound.ToString() + "Sound";
        AudioClip audioClip = GetSoundAudioClip(sound);

        if (audioClip != null)
        {
            GameObject soundGameObject = new GameObject(name);
            soundGameObject.transform.SetParent(gameObject.transform);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

            float cooldown = audioClip.length + interval;
            instance.StartCoroutine(Repeat(cooldown, audioSource, audioClip, soundGameObject, numberOfTimes));
        }
    }

    public static void StopRepeatSound(string sound, GameObject gameObject)
    {
        string name = sound.ToString() + "Sound";
        Transform soundTransform = gameObject.transform.Find(name);

        if (soundTransform != null)
        {
            Destroy(soundTransform.gameObject);
        }
    }

    public static void PlayMusic(string music)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = GetMusicAudioClip(music);
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    private static AudioClip GetSoundAudioClip(string sound)
    {
        foreach (SoundAudioClip soundAudioClip in instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }

        Debug.LogError("Sound " + sound + " not found");
        return null;
    }

    private static AudioClip GetMusicAudioClip(string music)
    {
        foreach (MusicAudioClip musicAudioClip in instance.musicAudioClipArray)
        {
            if (musicAudioClip.music == music)
            {
                return musicAudioClip.audioClip;
            }
        }

        Debug.LogError("Music " + music + " not found");
        return null;
    }

    private static IEnumerator Repeat(float interval, AudioSource audioSource, AudioClip audionClip, GameObject soundGameObject, int numberOfTimes)
    {
        int count = 0;

        while (soundGameObject != null)
        {
            audioSource.PlayOneShot(audionClip, MenuController.soundVolume);
            yield return new WaitForSeconds(interval);

            if (numberOfTimes != 0)
            {
                count++;
            }

            if (numberOfTimes != 0 && count == numberOfTimes)
            {
                if (soundGameObject != null)
                {
                    Destroy(soundGameObject);
                }

                break;
            }    
        }
    }

}
