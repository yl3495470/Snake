using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Music.
/// </summary>
public class Audio : MonoBehaviour
{
    public const string MUSIC_ACTIVE_KEY = "music_active";
    public const string SFX_ACTIVE_KEY = "sfx_active";
    private static Audio instance;

    /// <summary>
    /// The speaker.
    /// </summary>
    public AudioSource speaker1, speaker2;

    /// <summary>
    /// The music volume.
    /// </summary>
    public float musicVolume = 0.5f;

    /// <summary>
    /// The pause between replays.
    /// </summary>
    public float pauseBetweenReplays = 15f;

    /// <summary>
    /// The audio clips.
    /// </summary>
    public List<Clip> clips;

    [System.Serializable]
    /// <summary>
    /// Clip dictionary
    /// </summary>
    public class Clip
    {
        /// <summary>
        /// The key.
        /// </summary>
        public string key;

        /// <summary>
        /// The audio clip.
        /// </summary>
        public AudioClip audioClip;
    }

    private bool isSpeaker1 = true;
    private const float fadeDuration = 1f;
    public static Audio Instance;

    private static AudioSource currentSpeaker
    {
        get
        {
            return instance.isSpeaker1 ? instance.speaker1 : instance.speaker2;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Audio"/> is active.
    /// </summary>
    /// <value>
    /// <c>true</c> if is active; otherwise, <c>false</c>.
    /// </value>
    public static bool musicActive
    {
        get
        {
            return PlayerPrefs.GetInt(MUSIC_ACTIVE_KEY) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(MUSIC_ACTIVE_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
            instance.speaker1.mute = !value;
            instance.speaker2.mute = !value;
            if (value && !currentSpeaker.isPlaying)
            {
                currentSpeaker.Play();
            }
        }
    }

    public static bool sfxActive
    {
        get
        {
            return PlayerPrefs.GetInt(SFX_ACTIVE_KEY) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(SFX_ACTIVE_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
            AudioListener.volume = value ? 1f : 0f;
        }
    }

    private const int FIGHT_BGM_COUNT = 1;
    private const int BGM_COUNT = 1;

    void Awake()
    {
        Instance = this;
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(speaker1);
            DontDestroyOnLoad(speaker2);
        }

        // set music to ignore listener volume to separate it from sfx.
        speaker1.ignoreListenerVolume = true;
        speaker1.volume = musicVolume;
        speaker2.ignoreListenerVolume = true;
        speaker2.volume = musicVolume;
        speaker2.loop = true;



    }

    void Start()
    {
        //instance.speaker1.mute = !musicActive;
        //instance.speaker2.mute = !musicActive;
        //AudioListener.volume = sfxActive ? 1f : 0f;
        //Play(ResConfig.AUDIO_LOGIN_BGM);
        // ¶ÁÈ¡ÅäÖÃ
        if (PlayerPrefs.HasKey(SFX_ACTIVE_KEY))
        {
            AudioListener.volume = PlayerPrefs.GetInt(SFX_ACTIVE_KEY);
            sfxActive = AudioListener.volume == 1 ? true : false;
        }

        if (PlayerPrefs.HasKey(MUSIC_ACTIVE_KEY)) 
        {
            AudioListener.volume = PlayerPrefs.GetInt(MUSIC_ACTIVE_KEY);
            musicActive = AudioListener.volume == 1 ? true : false;
        }
        
    }

    void OnDestory()
    {
        Instance = null;
    }

    public static AudioClip GetClip(string key,bool isLoop= true)
    {
        AudioClip ac = null;
        Clip clip = instance.clips.Find(c => c.key == key);
        if (clip != null)
        {
            ac = clip.audioClip;
        }
        if (ac == null)
        {
            ac = Resourcer.GetAudio(key);
            if (ac == null)
            {
                Debug.LogError("cannot find audio clip with key " + key);
            }
            return ac;
        }
        else
        {
            return ac;
        }
    }

    public static void PlayClickOk()
    {
        PlayClip(ResConfig.AUDIO_OK);
    }

    public static void PlayClickClose()
    {
        PlayClip(ResConfig.AUDIO_CLOSE);
    }

    public static void PlayClickCancle()
    {
        PlayClip(ResConfig.AUDIO_CANCLE);
    }

    public static void PlayClickReturn()
    {
        PlayClip(ResConfig.AUDIO_RETURN);
    }

    public static void PlayHintNotice()
    {
        PlayClip(ResConfig.AUDIO_HINTNOTICE);
    }

    public static void PlayClip(string clip)
    {
        if (!string.IsNullOrEmpty(clip))
        {
            PlayClip(GetClip(clip));
        }
    }

    public static void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            instance.speaker1.PlayOneShot(clip);
        }
    }

    public static void Play(string music,bool isLoop = true)
    {
        Debug.Log(music);
        if (!string.IsNullOrEmpty(music))
        {
            Play(GetClip(music),isLoop);
        }
    }

    public static void PlayBGM()
    {
        instance.speaker2.Stop();
        Audio.Play(ResConfig.AUDIO_BGM + Random.Range(1, BGM_COUNT+1));      
    }

    public static void StopMusic() 
    {
        instance.speaker2.Stop();
    }

    public static void Play(AudioClip music,bool isLoop = true)
    {

        if (music == null)
        {
            Debug.LogError("music is null.");
            return;
        }

        //if (!musicActive)
        //{
            Debug.LogError("music is muted, and thus not played.");
            instance.speaker2.clip = music;
            instance.speaker2.loop = isLoop;
            instance.speaker2.Play();
            return;
        //}

    }

    //private IEnumerator ReplayAfterEnd (AudioClip music) {
    //    yield return new WaitForSeconds (music.length + pauseBetweenReplays);
    //    while (musicActive && !instance.speaker2.isPlaying && instance.speaker2.clip == music)
    //    {
    //        currentSpeaker.Play ();
    //        yield return new WaitForSeconds (music.length + pauseBetweenReplays);
    //    }
    //}

    //public static void Stop () {
    //    instance.StartCoroutine(instance.FadeOut(instance.speaker2));
    //}

    //private IEnumerator FadeOut (AudioSource speaker) {
    //    float countdown = fadeDuration;
    //    while (countdown > 0) {
    //        countdown -= Time.deltaTime;
    //        speaker.volume = Mathf.SmoothStep(0f, musicVolume, countdown/fadeDuration);
    //        yield return null;
    //    }
    //    speaker.Stop ();
    //    yield return null;
    //}
}
