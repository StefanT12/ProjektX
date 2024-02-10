//using Assets.Scripts.Input;
//using Assets.Utilities;
//using System;
//using System.Collections;
//using UnityEngine;

//namespace Assets.Scripts.Player
//{
//    public class CharacterController : MonoBehaviour, ICharacter
//    {
//        [SerializeField, RequireInterface(typeof(IControls))]
//        private UnityEngine.Object _brain;
//        public IControls Brain => _brain as IControls;
//        [field: SerializeField]
//        public Transform Transform { get; private set; }
//        public int Movement { get; set; } = 1;//starting from idle
//        public int Orientation { get; private set; }
//        public int IsGrounded { get; private set; }

//        [field: Header("Movement & Rotation")]

//        [field: SerializeField]
//        public float GravitationalAcceleration { get; set; } = 9.8f;
//        [field: SerializeField]
//        public float DampenedMovementSpeedFactor { get; set; } = 3f;

//        [field: SerializeField]
//        public float RadiansPerSecAdaptSpeed { get; set; } = 3f;
//        [field: SerializeField]

//        public CharacterMovementState[] CharacterMovementStates { get; set; } = new CharacterMovementState[4];

//        [field: Header("Physics")]
//        [field: SerializeField]
//        public float ToGroundRayLength { get; set; } = 2f;
//        [field: SerializeField]
//        public float ToGroundRayYOffset { get; set; } = 0.3f;
//        [field: SerializeField]
//        public LayerMask GroundLayers { get; set; }
//        [field: SerializeField]
//        public int BufferSize { get; set; } = 5;
//        [field: SerializeField]
//        public float GroundLimit { get; set; } = 0.8f;
//        [field: SerializeField]
//        public float GroundSystemLineWidthBleed { get; set; } = 1.2f;

//        private Rigidbody _rBody;
//        private Animator _anim;
//        private AnimatorHandler _animHandler;
//        private float _dampenedMovementSpeed;
//        private float _dampenedOrientation;
//        private RaycastHit[] _buff;
//        private float _groundedLineWidth;
//        private void Awake()
//        {
//            IEnumerator PostSimulationUpdate()
//            {
//                YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
//                while (true)
//                {
//                    yield return waitForFixedUpdate;

//                    LateFixedUpdate();
//                }
//            }

//            StartCoroutine(PostSimulationUpdate());
//        }
//        private void Start()
//        {
//            if (CharacterMovementStates == null || CharacterMovementStates.Length < 2)
//            {
//                throw new Exception("Set 4 different states please!");
//            }
            
//            Vector2 movementInput = new Vector2(0,0);
//            float lookInput = 0;
//            float sprintInput = 0;

//            void RecalculateMovementInput()
//            {
//                Movement = (int)(movementInput.y + 1 + (sprintInput * Mathf.Clamp01(movementInput.y)));
//                //_movementInputYMagnitude = Mathf.Abs(movementInput.y);
//            }
//            void RecalcuateOrientation()
//            {
//                Orientation = Mathf.Clamp((int)(lookInput + movementInput.x), -1, 1);
//            }

//            Brain.OnMove.AddListener(v =>
//            {
//                movementInput = v;
//                RecalculateMovementInput();
//                RecalcuateOrientation();
//            });
//            Brain.OnLook.AddListener(v => 
//            { 
//                lookInput = v.x;
//                RecalcuateOrientation();
//            });
//            Brain.OnSprint.AddListener(v => 
//            { 
//                sprintInput = v;
//                RecalculateMovementInput();
//            });

//            _rBody = GetComponent<Rigidbody>();
//            _animHandler = GetComponent<AnimatorHandler>();
//            _anim = GetComponent<Animator>();
//            _buff = new RaycastHit[BufferSize]; 
//            Transform = transform;
//            _groundedLineWidth = GetComponent<CapsuleCollider>().radius *2 * GroundSystemLineWidthBleed;
//        }

//        private int CalculateIsGrounded(byte totalRays)
//        {
//            for(byte r = 0; r < totalRays; r++)
//            {
//                int buffCount = Physics.RaycastNonAlloc(transform.position + transform.up * ToGroundRayYOffset - transform.forward * (_groundedLineWidth / 3 * r), -transform.up, _buff, ToGroundRayLength + ToGroundRayYOffset, GroundLayers);
//                float dist = ToGroundRayLength * 2;
//                for (byte i = 0; i < buffCount; i++)
//                {
//                    if (dist > _buff[i].distance)
//                    {
//                        dist = _buff[i].distance;
//                    }
//                }
//                dist -= ToGroundRayYOffset;
//                if(dist < GroundLimit)
//                {
//                    return 1;
//                }
//            }

//            return 0;
//        }
      
//        private void FixedUpdate()
//        {
//            IsGrounded = CalculateIsGrounded(2);

//            _dampenedMovementSpeed = Mathf.Lerp(_dampenedMovementSpeed, CharacterMovementStates[Movement].MovementSpeeds, DampenedMovementSpeedFactor * Time.fixedDeltaTime);

//            _dampenedOrientation = Mathf.Lerp(_dampenedOrientation, Orientation, CharacterMovementStates[Movement].OrientationSpeedFactors* Time.fixedDeltaTime);

//            _animHandler.UpdateMovementValues(_dampenedOrientation * Mathf.Clamp01(Movement + 1), _dampenedMovementSpeed, Time.fixedDeltaTime);//must be here as this statement makes Animator generate root motion
//            _anim.SetInteger("IsGrounded", IsGrounded);
//        }

//        //called in fixed update when Animator set to AnimatePhysics
//        private void OnAnimatorMove()
//        {
//            Vector3 vel = _anim.velocity;
//            vel.y = _rBody.velocity.y;
//            _rBody.velocity = vel;
//        }

//        private float _radiansPerSec;
//        //rotations must be set after the simulation but within the same framerate as FixedUpdate
//        private void LateFixedUpdate()
//        {
//            _radiansPerSec = Mathf.Lerp(_radiansPerSec, CharacterMovementStates[Movement].RadiansPerSecFactors, Time.deltaTime * RadiansPerSecAdaptSpeed);
//            _rBody.MoveRotation(transform.rotation * Quaternion.Lerp(_anim.deltaRotation, Quaternion.Euler(0f, _dampenedOrientation * _radiansPerSec * Time.fixedDeltaTime, 0f), 1));//_movementInputYMagnitude));
//        }
//    }
//}
