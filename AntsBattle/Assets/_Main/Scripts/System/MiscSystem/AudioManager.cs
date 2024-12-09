using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    private static AudioManager _main;

    public static AudioManager Main
    {
        set => _main = value;
        get
        {
            if (_main) return _main;
            _main = FindObjectOfType<AudioManager>(true);
            return _main;
        }
    }
    [SerializeField] private LevelValue isMuteAudio;
    [SerializeField] private AudioSource speaker;
    [SerializeField] private AudioSource bgm;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private List<AudioSource> loopClips = new List<AudioSource>();

    
    public enum SEType
    {
        None,
        Button,     // ボタン押した際
        SaleCard,   // カード売却ボタン
        PackOpen,   // パック開封
        CardSlide,  // パック開封からカードをスライド
        CardOpen,   // パックからカードを出した時
        CardOpenRare    // パックからカードを出した時のレア時
    }

    public bool IsActiveAudio => !IsMuteAudio;

    public bool IsMuteAudio
    {
        get => isMuteAudio.Value != 0;
        set => isMuteAudio.Value = value ? 1 : 0;
    }

    private void Awake()
    {
        bgm.volume = 0f; //突然音が出るのは驚くのでフェードイン
        var doVolume = DOVirtual.Float(0f, 1f, 10f, volume => { bgm.volume = volume; });
        doVolume.SetLink(gameObject);
        doVolume.onComplete = () => bgm.volume = 1f;

        UpdateMute();
    }

    /// <summary>
    /// SETypeからSEを再生させる
    /// </summary>
    public void PlaySE(SEType type)
    {
        if (!IsActiveAudio) return;
        
        int index = 0;
        float volume = 1f;
        switch (type)
        {
            case SEType.Button: // ボタン押した際
                index = 0;
                volume = 0.75f;
                break;
            case SEType.SaleCard:   // カード売却ボタン
                index = 8;
                break;
            case SEType.PackOpen:   // パック開封
                index = 7;
                break;
            case SEType.CardOpen:   // パックからカードを出した時
                index = 4;
                break;
            case SEType.CardOpenRare:   // パックからカードを出した時
                index = 5;
                break;
            case SEType.CardSlide:  // パック開封からカードをスライド
                index = 6;
                volume = 10f;
                break;
        }

        AudioSource.PlayClipAtPoint(clips[index], transform.position, volume);
    }

    public void PlayClip(int clipIndex)
    {
        if (!IsActiveAudio) return;
        AudioSource.PlayClipAtPoint(clips[clipIndex], transform.position);
    }

    public void PlayClipFromSpeaker(int clipIndex)
    {
        if (!IsActiveAudio) return;
        speaker.clip = clips[clipIndex];
        speaker.Play();
    }

    public void PlayLoopClip(int clipIndex, bool isMaxVolume = false)
    {
        if (!IsActiveAudio) return;

        var clip = loopClips[clipIndex];
        if (!clip.isPlaying) clip.Play();

        if (isMaxVolume)
        {
            clip.volume = 1f;
        }
        else
        {
            var vol = clip.volume;
            vol += Time.deltaTime * 10f;
            vol = Mathf.Clamp(vol, 0f, 1f);
            clip.volume = vol;
        }
    }


    private void Update()
    {
        foreach (var clip in loopClips)
        {
            var vol = clip.volume;
            if (IsActiveAudio)
            {
                vol -= Time.deltaTime;
                vol = Mathf.Max(0f, vol);
            }
            else
            {
                vol = 0f;
            }

            clip.volume = vol;
            if (clip.volume <= 0)
            {
                clip.Stop();
            }
        }
    }

    public void MuteForAds()
    {
        //リワードボタン押下時にすぐ音が消えてしまうとリワードボタン音が聞こえない
        var doMute = DOVirtual.DelayedCall(0.5f, () =>
        {
            var listeners = FindObjectsOfType<AudioListener>(true);
            foreach (var listener in listeners) listener.enabled = false;
            bgm.Pause();
        });
        doMute.SetLink(gameObject);
    }

    public void UpdateMute()
    {
        var listeners = FindObjectsOfType<AudioListener>(true);
        foreach (var listener in listeners) listener.enabled = IsActiveAudio;

        if (IsActiveAudio && !bgm.isPlaying) bgm.Play();
        if (!IsActiveAudio && bgm.isPlaying) bgm.Pause();
    }
}