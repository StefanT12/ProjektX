using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Utilities
{
    public static class ControllerExtensions
    {
        public static void PlayState(this Animator animator, string stateName, bool isInteracting, float crossFade)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool(nameof(isInteracting), isInteracting);
            animator.CrossFade(stateName, crossFade);
        }
    }
}
