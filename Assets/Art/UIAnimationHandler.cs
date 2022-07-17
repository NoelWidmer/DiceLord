using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationHandler : MonoBehaviour
{

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationState(int state)
    {
        animator.SetInteger ("state", state);
    }
}
