using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Input
{
    [CreateAssetMenu(fileName = "PlayerControls", menuName = "ScriptableObjects/PlayerControls")]
    public class PlayerControls : ScriptableObject, IControls
    {
        private global::PlayerControls _inputActions;

        [field: SerializeField]
        public UnityEvent<Vector2> OnMove { get; set; }
        
        [field: SerializeField]
        public UnityEvent<Vector2> OnLook { get; set; }
        public UnityEvent<float> OnSprint { get; set; }
        public UnityEvent<float> OnZoom { get; set; }

        private void OnEnable()
        {
            if(_inputActions == null)
            {
                _inputActions = new global::PlayerControls();
                _inputActions.PlayerMovement.Movement.performed += inputActions => OnMove?.Invoke(inputActions.ReadValue<Vector2>());
                _inputActions.PlayerMovement.Camera.performed += inputActions => OnLook?.Invoke(inputActions.ReadValue<Vector2>());
                _inputActions.PlayerActions.Sprint.performed += inputActions => OnSprint?.Invoke(inputActions.ReadValue<float>());
                _inputActions.PlayerMovement.CameraZoom.performed += inputActions => OnZoom?.Invoke(inputActions.ReadValue<float>());
            }
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }
    }
}
