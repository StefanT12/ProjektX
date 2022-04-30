using Assets.Scripts.Input;
using Assets.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField, RequireInterface(typeof(IControls))]
        private UnityEngine.Object _brain;
        public IControls Brain => _brain as IControls;
        [field: SerializeField]
        public Transform Transform { get; private set; }
        public int Movement { get; set; } = 1;//starting from idle
        public int Orientation { get; private set; }
        public bool IsGrounded { get; private set; }

        [field: Header("Movement & Rotation")]

        [field: SerializeField]
        public float GravitationalAcceleration { get; set; } = 9.8f;
        [field: SerializeField]
        public float DampenedMovementSpeedFactor { get; set; } = 3f;
        public float[] MovementSpeeds = new float[4];
        public float[] OrientationSpeedFactors= new float[4];

        [field: SerializeField]
        public float RadiansPerSecAdaptSpeed { get; set; } = 3f;
        public float[] RadiansPerSecFactors = new float[4];

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
        public Vector2 GroundLimit { get; set; }

        private Rigidbody _rBody;
        private Animator _anim;
        private AnimatorHandler _animHandler;
        private float _dampenedMovementSpeed;
        private float _dampenedOrientation;
        private RaycastHit[] _buff;
        private RaycastHit _closestGroundHit;
        private float _movementInputYMagnitude;

        private void Awake()
        {
            IEnumerator PostSimulationUpdate()
            {
                YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
                while (true)
                {
                    yield return waitForFixedUpdate;

                    LateFixedUpdate();
                }
            }

            StartCoroutine(PostSimulationUpdate());
        }
        private void Start()
        {
            if (MovementSpeeds == null || MovementSpeeds.Length < 4 || OrientationSpeedFactors == null || OrientationSpeedFactors.Length < 4)
            {
                throw new Exception("Set 4 different speeds for both movement and rotation please!");
            }
            
            Vector2 movementInput = new Vector2(0,0);
            float lookInput = 0;
            float sprintInput = 0;

            void RecalculateMovementInput()
            {
                Movement = (int)(movementInput.y + 1 + (sprintInput * Mathf.Clamp01(movementInput.y)));
                _movementInputYMagnitude = Mathf.Abs(movementInput.y);
            }
            void RecalcuateOrientation()
            {
                Orientation = Mathf.Clamp((int)(lookInput + movementInput.x), -1, 1);
            }

            Brain.OnMove.AddListener(v =>
            {
                movementInput = v;
                RecalculateMovementInput();
                RecalcuateOrientation();
            });
            Brain.OnLook.AddListener(v => 
            { 
                lookInput = v.x;
                RecalcuateOrientation();
            });
            Brain.OnSprint.AddListener(v => 
            { 
                sprintInput = v;
                RecalculateMovementInput();
            });

            _rBody = GetComponent<Rigidbody>();
            _animHandler = GetComponent<AnimatorHandler>();
            _anim = GetComponent<Animator>();
            _buff = new RaycastHit[BufferSize]; 
            Transform = transform;
        }

        private bool CalculateIsGrounded()
        {
            int buffCount = Physics.RaycastNonAlloc(transform.position  + transform.up * ToGroundRayYOffset, -transform.up, _buff, ToGroundRayLength + ToGroundRayYOffset, GroundLayers);
            _closestGroundHit.distance = ToGroundRayLength * 2;
            for (byte i = 0; i < buffCount; i++)
            {
                if (_closestGroundHit.distance > _buff[i].distance)
                {
                    _closestGroundHit = _buff[i];
                }
            }
            _closestGroundHit.distance -= ToGroundRayYOffset;
            return _closestGroundHit.distance < (IsGrounded ? GroundLimit.y : GroundLimit.x);
        }
      
        private void FixedUpdate()
        {
            IsGrounded = CalculateIsGrounded();

            _dampenedMovementSpeed = Mathf.Lerp(_dampenedMovementSpeed, MovementSpeeds[Movement], DampenedMovementSpeedFactor * Time.fixedDeltaTime);

            _dampenedOrientation = Mathf.Lerp(_dampenedOrientation, Orientation, OrientationSpeedFactors[Movement] * Time.fixedDeltaTime);

            _animHandler.UpdateMovementValues(_dampenedOrientation * Mathf.Clamp01(Movement + 1), _dampenedMovementSpeed, Time.fixedDeltaTime);//must be here as this statement makes Animator generate root motion, which will work in FixedUpdate
        }

        //called in fixed update when Animator set to AnimatePhysics
        private void OnAnimatorMove()
        {
            Vector3 vel = _anim.velocity;
            vel.y = _rBody.velocity.y;
            _rBody.velocity = vel;
            
        }

        private float _radiansPerSec;
        //rotations must be set after the simulation but within the same framerate as FixedUpdate
        private void LateFixedUpdate()
        {
            _radiansPerSec = Mathf.Lerp(_radiansPerSec, RadiansPerSecFactors[Movement], Time.deltaTime * RadiansPerSecAdaptSpeed);
            _rBody.MoveRotation(transform.rotation * Quaternion.Lerp(_anim.deltaRotation, Quaternion.Euler(0f, _dampenedOrientation * _radiansPerSec * Time.fixedDeltaTime, 0f), 1));//_movementInputYMagnitude));
        }
    }
}
