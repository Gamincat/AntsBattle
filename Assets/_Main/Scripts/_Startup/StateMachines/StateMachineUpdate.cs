using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace State.Machine
{
    public class StateMachineUpdate : StateMachine
    {
        private readonly Dictionary<int, List<StateAction>> _onEvents = new Dictionary<int, List<StateAction>>();
        private int _reservedMode;

        public delegate void ChangeMethod(int mode);

        public ChangeMethod OnChangeMode { get; set; }


        private void Awake()
        {
            //SetDummyEvents
            BaseInit();
            AddEvents(currentID, () => { }, () => { }, () => { });
        }


        private void Update()
        {
            ChangeModeImpl();
            _onEvents[currentID][(int)TriggerType.Stay]?.Invoke();
        }


        //Manage


        /// <summary>
        /// 各種モードの各状態へイベントを追加
        /// </summary>
        /// <param name="targetID">状態指定</param>
        public void AddEvents(int targetID, StateAction enter, StateAction stay, StateAction exit)
        {
            if (!_onEvents.ContainsKey(targetID))
            {
                _onEvents.Add(targetID, new StateAction[EventTypeCount].ToList());
            }

            if (enter != null) _onEvents[targetID][(int)TriggerType.Enter] += enter;
            if (stay != null) _onEvents[targetID][(int)TriggerType.Stay] += stay;
            if (exit != null) _onEvents[targetID][(int)TriggerType.Exit] += exit;
        }


        /// <summary>
        /// 状態遷移、必ずここから呼ぶように
        /// </summary>
        /// <param name="nextID">遷移先のモード名</param>
        public override void ChangeMode(int nextID)
        {
#if UNITY_EDITOR
            Debug.Log("ID:" + nextID);
#endif
            _reservedMode = nextID;
            OnChangeMode?.Invoke(_reservedMode);
        }


        private bool ChangeModeImpl()
        {
            if (_reservedMode == DummyID)
            {
                return false;
            }

            if (!_onEvents.ContainsKey(_reservedMode))
            {
                _reservedMode = DummyID;
            }

            var prevID = currentID;
            currentID = _reservedMode;

            _onEvents[prevID][(int)TriggerType.Exit]?.Invoke();
            _onEvents[currentID][(int)TriggerType.Enter]?.Invoke();

            _reservedMode = DummyID;
            return true;
        }
    }
}