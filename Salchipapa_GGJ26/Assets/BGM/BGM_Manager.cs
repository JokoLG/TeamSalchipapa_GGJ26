using UnityEngine;
using System.Collections.Generic;

public class BGM_Manager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource srcLoop;

    [Header("BGM Tracks")]
    public AudioClip norm;
    public AudioClip witch, ody, shark, boss;

    private Dictionary<string, AudioClip> sfx;

    void Awake()
    {
        srcLoop.playOnAwake = true;
        srcLoop.loop = true;

        sfx = new Dictionary<string, AudioClip>()
        {
            { "norm", norm },
            { "witch", witch },
            { "ody", ody },
            { "shark", shark },
            { "boss", boss },
        };
        PlayBGM("norm");
    }

    public void PlayBGM(string id, float volume = 0.3f)
    {
        if (!sfx.TryGetValue(id, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"BGM not found: {id}");
            return;
        }

        if (srcLoop.isPlaying && srcLoop.clip == clip) return;

        srcLoop.Stop();
        srcLoop.clip = clip;
        srcLoop.volume = volume;
        srcLoop.Play();
    }

    public void StopBGM()
    {
        srcLoop.Stop();
        srcLoop.volume = 1f;
    }
}