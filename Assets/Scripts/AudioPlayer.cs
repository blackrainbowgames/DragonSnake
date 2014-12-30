using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public bool Mute
    {
        get
        {
            return _mute;
        }
        set
        {
            _mute = value;
            _audioSource.volume = _mute ? 0 : 0.5f;
        }
    }

    private AudioSource _audioSource;
    private bool _mute;

    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        //PlayMusic();
    }

    private enum AudioTrack
    {
        Track1,
        Track2,
        Track3
    }

    private static List<AudioTrack> Tracks
    {
        get { return new List<AudioTrack> { AudioTrack.Track1, AudioTrack.Track2, AudioTrack.Track3 }; }
    }

    public void PlayMusic()
    {
        var track = Tracks[new CryptoRandom().Next(0, Tracks.Count)];

        StartCoroutine(PlayInGameNext(track, 0));
    }

    private IEnumerator PlayInGameNext(AudioTrack audioTrack, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        var clip = LoadAudioClip(audioTrack);

        _audioSource.clip = clip;
        _audioSource.loop = false;
        _audioSource.Play();

        var nextAudioTracks = Tracks;

        nextAudioTracks.Remove(audioTrack);

        var nextAudioTrack = nextAudioTracks[new CryptoRandom().Next(0, nextAudioTracks.Count)];

        while (nextAudioTrack == audioTrack)
        {
            nextAudioTrack = nextAudioTracks[new CryptoRandom().Next(0, nextAudioTracks.Count)];
        }

        StartCoroutine(PlayInGameNext(nextAudioTrack, clip.length));
    }

    private static AudioClip LoadAudioClip(AudioTrack audioTrack)
    {
        switch (audioTrack)
        {
            case AudioTrack.Track1:
                return (AudioClip) Resources.Load("Audio/Track1");
            case AudioTrack.Track2:
                return (AudioClip) Resources.Load("Audio/Track2");
            case AudioTrack.Track3:
                return (AudioClip) Resources.Load("Audio/Track3");
            default:
                throw new Exception();
        }
    }

    public void PlayAppleEaten()
    {
        PlayEffect("Audio/AppleEaten");
    }

    public void PlayUp()
    {
        PlayEffect("Audio/Up");
    }

    public void PlayPop()
    {
        PlayEffect("Audio/Pop");
    }

    public void PlayStageFailed()
    {
        PlayEffect("Audio/StageFailed");
    }

    public void PlayStageCompleted()
    {
        PlayEffect("Audio/StageCompleted");
    }

    public void PlayAchievement()
    {
        PlayEffect("Audio/Achievement");
    }

    private void PlayEffect(string path)
    {
        if (Mute) return;

        var clip = (AudioClip) Resources.Load(path);
        var audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(clip);
        Destroy(audioSource, clip.length);
    }
}