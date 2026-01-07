using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private bool playBgm;
    private int bgmIndex;

    private void Start()
    {
        PlayBGM(3);
    }

    private void Update()
    {
        if(!playBgm && BgmIsPlaying())
            StopAllBGM();
        else if (bgm[bgmIndex].isPlaying == false)
        {
            PlayRandomBGM();
        }
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