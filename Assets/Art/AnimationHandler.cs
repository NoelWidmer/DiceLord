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

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
        mask = GetComponent<SpriteMask>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mask.enabled = false;
        Flash(2, 0.2F);
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

    //true = Spiele Angriffsanimation, false = stope animation
    public void PlayOrStopAttack(bool state)
    {
        myAnimator.SetBool("attack", state);
    }
    
    //Muss aufgerufen werden wenn ein Charakter schaden nimt
    public void TakeDamage()
    {
        Flash(2, 0.2F);
    }

    //true = tot
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


    //For Player weapons
    //State 0 = without weapons
    //State 1 = sword
    //State 2 = Bow
    //State 3 = Staff
    public void setPlayerWeapon(int state)
    {
        myAnimator.SetInteger("state", state);
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
