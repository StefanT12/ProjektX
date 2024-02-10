using System;
using System.Collections;
using UnityEngine;

namespace Assets.Mechanics.Assets.Scripts.Player.Character
{
    [Serializable]
    public class CharacterTransitoryValues 
    {
        public Transform Transform { get; set; }
      

        public float DampenedMovementSpeed;
        public float DampenedOrientation;
        public RaycastHit[] Buff;
        public float GroundedLineWidth;
        internal float RadiansPerSec;

        public Rigidbody RBody;
        public Animator Anim;
        public AnimatorHandler AnimHandler;

    }
}