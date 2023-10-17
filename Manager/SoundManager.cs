using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoundManager : MonoSingleton<SoundManager>
{
    public enum ESound
    {
        BGM,
        FX,
    }

    [SerializeField]
    private AudioSource bgm = null;
    [SerializeField]
    private AudioSource fx = null;

    private Dictionary<string, AudioClip> soundDic = new Dictionary<string, AudioClip>();

    public AudioSource BGM => bgm;
    public AudioSource FX => fx;

    public Dictionary<string, AudioClip> SoundDic => soundDic;

    public void SetSoundDic(Dictionary<string, AudioClip> soundDic)
    {
        this.soundDic = soundDic;
    }

    public void SetVolume(ESound type, float volume)
    {
        switch (type)
        {
            case ESound.BGM:
                bgm.volume = volume;
                break;
            case ESound.FX:
                fx.volume = volume;
                break;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        bgm.Stop();
        bgm.clip = clip;
        bgm.Play();
    }

    public void PauseBGM(bool isPause)
    {
        if (isPause == true)
        {
            bgm.Pause();
        }
        else
        {
            bgm.Play();
        }
    }

    public void OnApplicationPause(bool pause)
    {
        PauseBGM(pause);
    }

    public void PlayFx(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }

        fx.PlayOneShot(clip);
    }

}
