using Assets.Mechanics.Assets.Scripts.Manager;
using Assets.Scripts.Input;
using Assets.Utilities;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Mechanics.Assets.Scripts.Player.Character
{
    public class CharacterContainer : MonoBehaviour, IContainer
    {
        [SerializeField, RequireInterface(typeof(IControls))]
        private UnityEngine.Object _brain;
        public IControls Brain => _brain as IControls;

        [field: Header("Movement & Rotation")]

        [field: SerializeField]
        public float GravitationalAcceleration { get; set; } = 9.8f;
        [field: SerializeField]
        public float DampenedMovementSpeedFactor { get; set; } = 3f;

        [field: SerializeField]
        public float RadiansPerSecAdaptSpeed { get; set; } = 3f;
        [field: SerializeField]

        public CharacterMovementState[] CharacterMovementStates { get; set; } = new CharacterMovementState[4];

        [field: Header("Physics")]
        [field: SerializeField]
        public float ToGroundRayLength { get; set; } = 2f;
        [field: SerializeField]
        public float ToGroundRayYOffset { get; set; } = 0.3f;
        [field: SerializeField]
        public LayerMask GroundLayers { get; set; }
        [field: SerializeField]
        public int BufferSize { get; set; } = 5;
        [field: SerializeField]
        public float GroundLimit { get; set; } = 0.8f;
        [field: SerializeField]
        public float GroundSystemLineWidthBleed { get; set; } = 1.2f;
        public GameObject ThisGO { get; set; }

        private void Awake()
        {
            ThisGO = gameObject;
        }
    }
}