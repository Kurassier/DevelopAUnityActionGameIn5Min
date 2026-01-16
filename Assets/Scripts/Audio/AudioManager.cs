
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Audio;

//public class AudioManager : MonoBehaviour
//{
//    static AudioManager main;


//    private AudioMixer audioMixer;

//    private List<AudioSource> soundSources;
//    private static AudioSource musicSource;

//    [SerializeField] private AudioClip[] soundClips;
//    [SerializeField] private AudioClip[] musicClips;

//    public string musicName = "Battle_BGM";

//    private void Awake()
//    {
//        if (main != null) Destroy(main.gameObject);
//        main = this;

//        audioMixer = Resources.Load<AudioMixer>("AudioMixer");

//        soundClips = Resources.LoadAll<AudioClip>("Sound");
//        musicClips = Resources.LoadAll<AudioClip>("Music");

//        soundSources = new List<AudioSource>();

//        //新建几个空的音效播放器
//        for (int i = 0; i < 5; i++)
//            NewSoundSource();

//        //播放背景音乐
//        PlayMusic(musicName, 0.3f, 1f);
//    }

//    AudioSource NewSoundSource()
//    {
//        AudioSource soundSource = gameObject.AddComponent<AudioSource>();

//        GameObject sound = new GameObject("Sound Source");
//        sound.transform.parent = transform;
//        soundSource = sound.AddComponent<AudioSource>();
//        soundSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];
//        soundSource.playOnAwake = false;
//        soundSource.loop = false;

//        soundSources.Add(soundSource);
//        return soundSource;
//    }

//    static void NewMusicSource()
//    {
//        GameObject music = new GameObject("Music Source");
//        DontDestroyOnLoad(music);
//        musicSource = music.AddComponent<AudioSource>();
//        musicSource.outputAudioMixerGroup = main.audioMixer.FindMatchingGroups("Music")[0];
//        musicSource.playOnAwake = false;
//        musicSource.loop = true;
//    }


//    public static void SetVolumeMaster(float volume)
//    {
//        volume = VolumeReflection(volume);
//        main.audioMixer.SetFloat("Volume_Master", volume);
//    }

//    public static void SetVolumeSound(float volume)
//    {
//        volume = VolumeReflection(volume);
//        main.audioMixer.SetFloat("Volume_Sound", volume);
//    }

//    public static void SetVolumeMusic(float volume)
//    {
//        volume = VolumeReflection(volume);
//        main.audioMixer.SetFloat("Volume_Music", volume);
//    }

//    static float VolumeReflection(float volume)
//    {
//        if (volume != 0)
//            return volume * 40 - 20;
//        else
//            return -80;
//    }

//    public static void PlaySound(string soundName, float volume = -1f, float pitch = -1f)
//    {
//        if (soundName == "") return;

//        if (volume == -1) volume = Random.Range(0.9f, 1.1f);
//        if (pitch == -1) pitch = Random.Range(0.9f, 1.1f);

//        AudioClip clip = System.Array.Find(main.soundClips, x => x.name == soundName);

//        if (clip != null)
//        {
//            //搜索有无空闲的音效播放器
//            AudioSource soundSource = null;
//            foreach (AudioSource source in main.soundSources)
//                if (!source.isPlaying) soundSource = source;
//            //没有则新增播放器
//            if (soundSource == null)
//                soundSource = main.NewSoundSource();

//            soundSource.pitch = pitch;
//            soundSource.PlayOneShot(clip, volume);
//        }
//        else
//        {
//            Debug.LogError("Sound not found: " + soundName);
//        }
//    }

//    public static void PlayMusic(string musicName, float volume = 1f, float pitch = 1f)
//    {
//        if (musicName == "")
//        {
//            if (musicSource != null)
//                Destroy(musicSource.gameObject);
//            return;
//        }

//        bool needRestart = true;
//        AudioClip clip = System.Array.Find(main.musicClips, x => x.name == musicName);
//        if (clip != null)
//        {
//            if (musicSource != null)
//            {
//                if (musicSource.clip == clip)
//                {
//                    //如果正在播放当前BGM，不再重新播放
//                    needRestart = false;
//                }
//                else
//                {
//                    //如果正在播放BGM，但是BGM不对，则重新设置BGM
//                }
//            }
//            else
//            {
//                //如果播放器不存在，则先生成播放器再设置BGM
//                NewMusicSource();
//            }

//            musicSource.clip = clip;
//            musicSource.volume = volume;
//            musicSource.pitch = pitch;
//            musicSource.loop = true;
//            if (needRestart)
//                musicSource.Play();
//        }
//        else
//        {
//            Debug.LogError("Music not found: " + musicName);
//        }
//    }
//    public static void ConfigMusic(float volume = 1f, float pitch = 1f)
//    {
//        musicSource.volume = volume;
//        musicSource.pitch = pitch;
//    }

//    public static void StopMusic()
//    {
//        musicSource.Stop();
//    }

//    public static void PauseMusic()
//    {
//        musicSource.Pause();
//    }

//    public static void UnpauseMusic()
//    {
//        musicSource.UnPause();
//    }
//}
