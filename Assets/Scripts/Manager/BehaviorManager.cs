using Assets.Scripts.Input;
using Assets.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Assets.Mechanics.Assets.Scripts.Manager
{
    public interface IContainer
    {
        public GameObject ThisGO { get; set; }
    }
    public abstract class BehaviorManager<ContainerT, DumpT> : MonoBehaviour where ContainerT : MonoBehaviour, IContainer where DumpT : new()
    {
        internal List<(ContainerT container, DumpT dump)> Entities { get; private set; }

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

        internal void RefreshContainers()
        {
            Entities = GameObject.FindObjectsOfType<ContainerT>().Select(x => (x, new DumpT())).ToList();
        }

        internal virtual void LateFixedUpdate()
        {
            
        }
    }
}