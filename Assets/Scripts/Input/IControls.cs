using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Input
{
    public interface IControls
    {
        public UnityEvent<Vector2> OnMove { get; set; }
        public UnityEvent<Vector2> OnLook { get; set; }

        public UnityEvent<float> OnSprint { get; set; }

        public UnityEvent<float> OnZoom { get; set; }
    }
}
