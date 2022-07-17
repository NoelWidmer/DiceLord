using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void PlayParallelSound(this MonoBehaviour behaviour, ref List<AudioSource> sources, AudioClip clip)
    {
        var availableSrc = sources.FirstOrDefault(src => src.isPlaying == false);

        if (availableSrc == null)
        {
            availableSrc = behaviour.gameObject.AddComponent<AudioSource>();
            sources.Add(availableSrc);

            availableSrc.spatialBlend = 1f;
        }

        availableSrc.clip = clip;
        availableSrc.Play();
    }
}
