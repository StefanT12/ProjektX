using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    private Animator _anim;
    int _vertical, _horizontal, _isGrounded;

    public void Start()
    {
        _anim = GetComponent<Animator>();
        _vertical = Animator.StringToHash("Vertical");
        _horizontal = Animator.StringToHash("Horizontal");
        _isGrounded = Animator.StringToHash("IsGrounded");

    }

    public void UpdateMovementValues(float verticalMovement, float horizontalMovement, int isGrounded, float delta)
    {
        _anim.SetFloat(_vertical, verticalMovement);
        _anim.SetFloat(_horizontal, horizontalMovement, 0.1f, delta);//best explanation of dampTime here: https://answers.unity.com/questions/611667/damptime-and-deltatime-in-setfloat-parameters.html
        _anim.SetInteger(_isGrounded, isGrounded);
    }

    public void PlayTargetState(string targetStateName, bool isInteracting)
    {
        _anim.SetBool(nameof(isInteracting), isInteracting);
        _anim.CrossFade(targetStateName, 0.2f);
    }
}
