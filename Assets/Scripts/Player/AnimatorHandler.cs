using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    private Animator _anim;
    int _vertical, _horizontal;

    public void Start()
    {
        _anim = GetComponent<Animator>();
        _vertical = Animator.StringToHash("Vertical");
        _horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateMovementValues(float verticalMovement, float horizontalMovement, float delta)
    {
        _anim.SetFloat(_vertical, verticalMovement, 0.1f, delta);
        _anim.SetFloat(_horizontal, horizontalMovement, 0.1f, delta);
    }

    public void PlayTargetState(string targetStateName, bool isInteracting)
    {
        _anim.SetBool(nameof(isInteracting), isInteracting);
        _anim.CrossFade(targetStateName, 0.2f);
    }
}
