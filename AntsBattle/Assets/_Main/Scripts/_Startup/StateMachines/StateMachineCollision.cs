using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace State.Machine
{

    public delegate void CollisionMethod(Collision other);

    public class StateMachineCollision : StateMachine
    {


        private readonly Dictionary<int, List<CollisionMethod>> _onEvents =
            new Dictionary<int, List<CollisionMethod>>();


        private void Awake()
        {
            //SetDummyEvents
            BaseInit();
            AddEvents(currentID, (b) =>
            {
            }, (b) =>
            {
            }, (b) =>
            {
            });
        }


        private void OnCollisionEnter(Collision other)
        {
            _onEvents[currentID][(int) TriggerType.Enter]?.Invoke(other);
        }


        private void OnCollisionStay(Collision other)
        {
            _onEvents[currentID][(int) TriggerType.Stay]?.Invoke(other);
        }


        private void OnCollisionExit(Collision other)
        {
            _onEvents[currentID][(int) TriggerType.Exit]?.Invoke(other);
        }


        //Manage


        public void AddEvents(int targetID, CollisionMethod enter, CollisionMethod stay, CollisionMethod exit)
        {
            if (!_onEvents.ContainsKey(targetID))
            {
                _onEvents.Add(targetID, new CollisionMethod[EventTypeCount].ToList());
            }

            if (enter != null) _onEvents[targetID][(int) TriggerType.Enter] += enter;
            if (stay != null) _onEvents[targetID][(int) TriggerType.Stay] += stay;
            if (exit != null) _onEvents[targetID][(int) TriggerType.Exit] += exit;
        }


        public override void ChangeMode(int nextID)
        {
            if (!_onEvents.ContainsKey(nextID))
            {
                nextID = DummyID;
            }

            currentID = nextID;
        }
    }
}
