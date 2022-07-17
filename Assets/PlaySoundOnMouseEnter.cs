using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnMouseEnter : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip HoverSound;

    private List<AudioSource> _sources = new();

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.PlayParallelSound(ref _sources, HoverSound, false);
    }
}
