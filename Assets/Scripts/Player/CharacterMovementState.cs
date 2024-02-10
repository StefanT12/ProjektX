using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMovementState", menuName = "ScriptableObjects/CharacterMovementState")]
public class CharacterMovementState: ScriptableObject
{
    [field: SerializeField]
    public float MovementSpeeds { get; set; }
    
    [field: SerializeField]
    public float OrientationSpeedFactors { get; set; }
    
    [field: SerializeField]
    public float RadiansPerSecFactors { get; set; }
}