using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Singleton
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }

    private AudioSource audioPlayer;

    public enum Sound
    {
        BGM,
        P1_Effect,
        P2_Effect,
        UI,
        MaxCount,  //Length
    }

    //音を鳴らすAudisoSourceを複数所持
    [SerializeField] AudioSource[] audioSources = new AudioSource[(int)Sound.MaxCount];
    //directoryと一緒にサウンドリソースをDictionaryに保存
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public void Clear()
    {
        // すべてのAudioSource Stop
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        // サウンドリソース削除
        audioClips.Clear();
    }

    //directoryを受けてこのスクリプトの内で再生させる
    public void Play(string path, Sound type = Sound.P1_Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type); //リソースを取得
        Play(audioClip, type, pitch);
    }

    //サウンド再生（リソースあり）
    private void Play(AudioClip audioClip, Sound type = Sound.P1_Effect, float pitch = 1.0f)
    {
        if (audioClip == null || audioSources.Length <= (int)type)
            return;

        AudioSource audioSource = audioSources[(int)type];

        if (type == Sound.BGM) //Background music
        {

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    //サウンドリソース取得
    AudioClip GetOrAddAudioClip(string path, Sound type = Sound.P1_Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // directoryに"Sounds/"が含まれていない場合追加

        AudioClip audioClip = null;

        //すでに取得済みのサウンドか確認
        if (audioClips.TryGetValue(path, out audioClip) == false)
        {
            audioClip = Resources.Load<AudioClip>(path);
            audioClips.Add(path, audioClip);
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }
}


