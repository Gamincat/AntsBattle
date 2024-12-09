using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void TriggerMethod(Collider other);

namespace State.Machine
{

    public class StateMachineTrigger : StateMachine
    {
        private readonly Dictionary<int, List<TriggerMethod>> _onEvents = new Dictionary<int, List<TriggerMethod>>();


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


        private void OnTriggerEnter(Collider other)
        {
            _onEvents[currentID][(int) TriggerType.Enter]?.Invoke(other);
        }


        private void OnTriggerStay(Collider other)
        {
            _onEvents[currentID][(int) TriggerType.Stay]?.Invoke(other);
        }


        private void OnTriggerExit(Collider other)
        {
            _onEvents[currentID][(int) TriggerType.Exit]?.Invoke(other);
        }


        //Manage


        public void AddEvents(int targetID, TriggerMethod enter, TriggerMethod stay, TriggerMethod exit)
        {
            if (!_onEvents.ContainsKey(targetID))
            {
                _onEvents.Add(targetID, new TriggerMethod[EventTypeCount].ToList());
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
