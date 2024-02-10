using Assets.Scripts.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public interface ICharacter
    {
        //IControls Brain { get; }
        Transform Transform { get; }
        int Orientation { get; }
        int Movement { get; }
        int IsGrounded { get; }
    }
}
