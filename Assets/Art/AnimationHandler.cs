using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] List<AnimationClip> clipList = new List<AnimationClip>();
    [SerializeField] List<GameObject> particals = new List<GameObject>();
    
    Animator myAnimator;
    GameObject myParticleSystem;

    SpriteMask mask;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        mask = GetComponent<SpriteMask>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mask.enabled = false;
    }

    public void PlayOrStopMove(bool state)
    {
        if (state)
        {
            //myParticleSystem = Instantiate(particals[0]);
            //myParticleSystem.transform.parent = transform;
        }
        //else
            //Destroy(myParticleSystem);
    }

    public void PlayOrStopAttack(bool state)
    {
        myAnimator.SetBool("attack", state);
    }
       
    public void TakeDamage()
    {
        Flash(2, 0.2F);
    }

    public void PlayOrStopDeath(bool state)
    {
        myAnimator.SetBool("death", state);
    }

    public void StopIdle()
    {
        //not yet needed
    }

    void FixedUpdate()
    {
        mask.sprite = spriteRenderer.sprite;
    }

    public void Flash(int amount, float duration)
    {
        StartCoroutine(Reset(duration, amount, 0));
    }


    IEnumerator Reset(float waitTime, float amount, int curentamount)
    {
        yield return new WaitForSeconds(waitTime);
        if (curentamount != amount)
        {
            if (mask.enabled)
                mask.enabled = false;
            else
                mask.enabled = true;
            StartCoroutine(Reset(waitTime, amount, ++curentamount));
        }
    }

}
