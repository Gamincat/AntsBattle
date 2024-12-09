using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace State.Machine
{

    public enum TriggerType
    {
        Enter,
        Stay,
        Exit
    }



    public class StateMachine : MonoBehaviour
    {
        public delegate void StateAction();

        protected int EventTypeCount => Enum.GetNames(typeof(TriggerType)).Length;
        protected readonly int DummyID = -1;
        public int currentID { get; set; }


        protected void BaseInit()
        {
            currentID = DummyID;
        }


        public virtual void ChangeMode(int nextID)
        {
        }
    }
}
