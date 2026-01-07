using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private bool playBgm;
    [SerializeField] private int bgmIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayBGM(3);
    }

    private void Update()
    {
        if(!playBgm && BgmIsPlaying())
            StopAllBGM();

        if (playBgm && bgm[bgmIndex].isPlaying == false)
        {
            PlayRandomBGM();
        }
    }

    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = 0.85f, float maxPitch = 1.1f)
    {
        if(sfx == null) return;
        
        float pitch = Random.Range(minPitch, maxPitch);
        sfx.pitch = pitch;
        sfx.Play();
    }
    
    private void PlayBGM(int index)
    {
        StopAllBGM();
        bgmIndex = index;
        bgm[index].Play();
    }

    private void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    [ContextMenu("Play random music")]
    public void PlayRandomBGM()
    {
        StopAllBGM();
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }
        return false;
    }
}