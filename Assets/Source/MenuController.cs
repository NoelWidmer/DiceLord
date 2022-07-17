using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public AudioClip ClickSound;

    public void ExitGame()
    {
        Click();
        StartCoroutine(Delay(() => Application.Quit()));
    }

    public void StartGame()
    {
        Click();
        StartCoroutine(Delay(() => SceneManager.LoadScene(gameObject.scene.buildIndex + 1)));
    }

    IEnumerator Delay(Action action)
    {
        yield return new WaitForSeconds(.25f);
        action();
    }

    private List<AudioSource> _sources = new();

    private void Click()
    {
        this.PlayParallelSound(ref _sources, ClickSound, false);
    }
}
