using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class SoundManager : IStartable
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>(); //Caching

    private readonly ResourceManager _resourceManager;
    public SoundManager(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }
    public void Start()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            UnityEngine.Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string key, Define.Sound type = Define.Sound.Effect, float pitch = 1f)
    {
        key = $"Sounds/{key}";
        GetOrAddAudioClipAsync(key, type, (audioClip) =>
        {
            Play(audioClip, type, pitch);
        });
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    private void GetOrAddAudioClipAsync(string key, Define.Sound type = Define.Sound.Effect, Action<AudioClip> completed = null)
    {
        if (type == Define.Sound.Bgm)
        {
            _resourceManager.LoadAsync<AudioClip>(key, (audioClip) =>
            {
                completed?.Invoke(audioClip);
            });
        }
        else
        {
            AudioClip audioClip = null;
            if (_audioClips.TryGetValue(key, out audioClip) == false)
            {
                _resourceManager.LoadAsync<AudioClip>(key, (clip) =>
                {
                    _audioClips.Add(key, clip);
                    completed?.Invoke(clip);
                });
            }
            else
            {
                completed?.Invoke(audioClip);
            }
        }
    }
}
