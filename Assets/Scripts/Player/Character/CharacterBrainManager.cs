using Assets.Mechanics.Assets.Scripts.Manager;
using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.Mechanics.Assets.Scripts.Player.Character
{
    /// <summary>
    /// Character motion states
    /// </summary>
    public class MotionStates
    {
        public int Movement { get; set; }
        public int Orientation { get; set; }
        public int IsGrounded { get; set; }
    }
    /// <summary>
    /// Constantly changing variables
    /// </summary>
    public class MotionValues
    {
        public float DampenedMovementSpeed { get; set; }
        public float DampenedOrientation { get; set; }
        public float RadiansPerSec { get; set; }
    }
    public class Components
    {
        public Transform Transform { get; set; }
        public Rigidbody RBody;
        public Animator Anim;
        public AnimatorHandler AnimHandler;
    }

    /// <summary>
    /// Buffers & one time variables
    /// </summary>
    public class Transitory
    {
        public RaycastHit[] Buff;
        public float GroundedLineWidth { get; set; }
    }

    /// <summary>
    /// Variablesd needed by the system to operate
    /// </summary>
    public class CharacterDump
    {
        public Transitory Transitories { get; set; }

        public Components Components { get; set; }

        public MotionStates MotionStates { get; set; }

        public MotionValues MotionValues { get; set; }
     
    }
    public class CharacterBrainManager : BehaviorManager<CharacterContainer, CharacterDump>
    {
        private int CalculateIsGrounded(int totalRays, CharacterContainer container, CharacterDump dump)
        {
            for (byte r = 0; r < totalRays; r++)
            {
                int buffCount = Physics.RaycastNonAlloc(dump.Components.Transform.position + dump.Components.Transform.up * container.ToGroundRayYOffset - dump.Components.Transform.forward * (dump.Transitories.GroundedLineWidth / 3 * r), -dump.Components.Transform.up, dump.Transitories.Buff, container.ToGroundRayLength + container.ToGroundRayYOffset, container.GroundLayers);
                float dist = container.ToGroundRayLength * 2;
                for (byte i = 0; i < buffCount; i++)
                {
                    if (dist > dump.Transitories.Buff[i].distance)
                    {
                        dist = dump.Transitories.Buff[i].distance;
                    }
                }
                dist -= container.ToGroundRayYOffset;
                if (dist < container.GroundLimit)
                {
                    return 1;
                }
            }

            return 0;
        }

        public void MapInputToMotionStates(CharacterContainer container, CharacterDump dump)
        {
            dump.MotionStates = new MotionStates 
            {
                Movement = 1//start in idle
            };
            dump.MotionValues = new MotionValues();

            Vector2 movementInput = new Vector2(0, 0);
            float lookInput = 0;
            float sprintInput = 0;

            void RecalculateMovementInput()
            {
                dump.MotionStates.Movement = (int)(movementInput.y + 1 + (sprintInput * Mathf.Clamp01(movementInput.y)));
                //_movementInputYMagnitude = Mathf.Abs(movementInput.y);
            }
            void RecalcuateOrientation()
            {
                dump.MotionStates.Orientation = Mathf.Clamp((int)(lookInput + movementInput.x), -1, 1);
            }

            container.Brain.OnMove.AddListener(v =>
            {
                movementInput = v;
                RecalculateMovementInput();
                RecalcuateOrientation();
            });
            container.Brain.OnLook.AddListener(v =>
            {
                lookInput = v.x;
                RecalcuateOrientation();
            });
            container.Brain.OnSprint.AddListener(v =>
            {
                sprintInput = v;
                RecalculateMovementInput();
            });
        }

        private void UpdateMotionValuesFromMotionStates(CharacterContainer container, MotionStates motionStates, MotionValues motionValues, AnimatorHandler animatorHandler)
        {
            motionValues.DampenedMovementSpeed = Mathf.Lerp(motionValues.DampenedMovementSpeed, container.CharacterMovementStates[motionStates.Movement].MovementSpeeds, container.DampenedMovementSpeedFactor * Time.fixedDeltaTime);

            motionValues.DampenedOrientation = Mathf.Lerp(motionValues.DampenedOrientation, motionStates.Orientation, container.CharacterMovementStates[motionStates.Movement].OrientationSpeedFactors * Time.fixedDeltaTime);

            animatorHandler.UpdateMovementValues(motionValues.DampenedOrientation * Mathf.Clamp01(motionStates.Movement + 1), motionValues.DampenedMovementSpeed, motionStates.IsGrounded, Time.fixedDeltaTime);//must be here as this statement makes Animator generate root motion
        }

        public void UpdateMotion(CharacterDump dump)
        {
            Vector3 vel = dump.Components.Anim.velocity;
            vel.y = dump.Components.RBody.velocity.y;
            dump.Components.RBody.velocity = vel;
        }

        public void UpdateRotation(CharacterContainer container, CharacterDump dump)
        {
            dump.MotionValues.RadiansPerSec = Mathf.Lerp(dump.MotionValues.RadiansPerSec, container.CharacterMovementStates[dump.MotionStates.Movement].RadiansPerSecFactors, Time.deltaTime * container.RadiansPerSecAdaptSpeed);
            dump.Components.RBody.MoveRotation(dump.Components.Transform.rotation * Quaternion.Lerp(dump.Components.Anim.deltaRotation, Quaternion.Euler(0f, dump.MotionValues.DampenedOrientation * dump.MotionValues.RadiansPerSec * Time.fixedDeltaTime, 0f), 1));//_movementInputYMagnitude));
        }
        public void Start()
        {
            RefreshContainers();

            foreach(var entity in  Entities)
            {
                entity.dump.Transitories = new Transitory
                {
                    Buff = new RaycastHit[entity.container.BufferSize],
                    GroundedLineWidth = entity.container.ThisGO.GetComponent<CapsuleCollider>().radius * 2 * entity.container.GroundSystemLineWidthBleed
                };

                entity.dump.Components = new Components
                {
                    Transform = entity.container.ThisGO.transform,
                    Anim = entity.container.ThisGO.GetComponent<Animator>(),
                    RBody = entity.container.ThisGO.GetComponent<Rigidbody>(),
                    AnimHandler = entity.container.ThisGO.GetComponent<AnimatorHandler>(),
                };

                MapInputToMotionStates(entity.container, entity.dump);
            }
        }

        public void FixedUpdate()
        {
            foreach (var entity in Entities)
            {
                entity.dump.MotionStates.IsGrounded = CalculateIsGrounded(2, entity.container, entity.dump);

                UpdateMotionValuesFromMotionStates(entity.container, entity.dump.MotionStates, entity.dump.MotionValues, entity.dump.Components.AnimHandler);
            }
        }

        public void OnAnimatorMove()
        {
            foreach (var entity in Entities)
            {
                UpdateMotion(entity.dump);
            }
        }

        internal override void LateFixedUpdate()
        {
            foreach (var entity in Entities)
            {
                UpdateRotation(entity.container, entity.dump);
            }
        }
    }
}