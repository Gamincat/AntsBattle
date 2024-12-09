using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiTapInputter : MonoBehaviour
{
    [SerializeField] private float limit = 0.5f;

    public Action<TouchState> OnEnter { get; set; }
    public Action<TouchState> OnStay { get; set; }
    public Action<TouchState> OnExit { get; set; }

    public float Aspect { get; set; }

    private Dictionary<int, TouchState> TouchStates { get; set; } = new Dictionary<int, TouchState>();

    private KeyCode DualFlag { get; set; } = KeyCode.Escape;

    private float Limit => limit;


    private void Awake()
    {
        Aspect = Mathf.Max(Screen.width, Screen.height);
    }


    private void OnDisable()
    {
        foreach (var kv in TouchStates)
        {
            var state = kv.Value;
            state.Phase.Value = TouchPhase.Ended;
            OnExit?.Invoke(state);
        }
    }


    private void Update()
    {
#if UNITY_EDITOR

        var mPos = Input.mousePosition;

        var aPhase = TouchPhase.Canceled;
        if (Input.GetMouseButtonDown(0)) aPhase = TouchPhase.Began;
        else if (Input.GetMouseButton(0)) aPhase = TouchPhase.Moved;
        else if (Input.GetMouseButtonUp(0)) aPhase = TouchPhase.Ended;
        InputForEditor(0, mPos, aPhase);


        //左手用デバッグ

        var bPhase = TouchPhase.Canceled;
        if (Input.GetMouseButtonDown(0)) bPhase = TouchPhase.Began;
        else if (Input.GetMouseButton(0)) bPhase = TouchPhase.Moved;
        else if (Input.GetMouseButtonUp(0)) bPhase = TouchPhase.Ended;

        if (Input.GetKey(KeyCode.Q)) DualFlag = KeyCode.Q;
        else if (Input.GetKey(KeyCode.W)) DualFlag = KeyCode.W;
        else if (Input.GetKey(KeyCode.E)) DualFlag = KeyCode.E;
        else if (Input.GetKey(KeyCode.R)) DualFlag = KeyCode.R;

        var isEnter = Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E) ||
                      Input.GetKeyDown(KeyCode.R) ||
                      Input.GetMouseButtonDown(0);
        if (isEnter && aPhase == TouchPhase.Moved) bPhase = TouchPhase.Began;


        var reverseStartPos = TouchStates[0].StartScreenPos;
        reverseStartPos.x = Screen.width - TouchStates[0].StartScreenPos.x;

        switch (DualFlag)
        {
            case KeyCode.Q:
                mPos = reverseStartPos + (isEnter ? Vector2.zero : TouchStates[0].OffsetScreen);
                InputForEditor(1, mPos, bPhase);
                break;
            case KeyCode.W:
                mPos = reverseStartPos - (isEnter
                    ? Vector2.zero
                    : new Vector2(TouchStates[0].OffsetScreen.x, -TouchStates[0].OffsetScreen.y));
                InputForEditor(1, mPos, bPhase);
                break;
            case KeyCode.E:
                mPos = reverseStartPos - (isEnter ? Vector2.zero : TouchStates[0].OffsetScreen);
                InputForEditor(1, mPos, bPhase);
                break;
            case KeyCode.R:
                mPos = reverseStartPos - (isEnter
                    ? Vector2.zero
                    : new Vector2(-TouchStates[0].OffsetScreen.x, TouchStates[0].OffsetScreen.y));
                InputForEditor(1, mPos, bPhase);
                break;
        }

        if (!Input.GetMouseButton(0)) DualFlag = KeyCode.Escape;


#else
        InputForDevice();
#endif
    }


    /// <summary>
    /// エディタ上での入力
    /// </summary>
    private void InputForEditor(int id, Vector2 pos, TouchPhase phase)
    {
        if (!TouchStates.ContainsKey(id))
        {
            var newState = gameObject.AddComponent<TouchState>();
            newState.Limit = limit;
            TouchStates.Add(id, newState);
        }

        var state = TouchStates[id];

        state.Phase.Value = phase;
        switch (phase)
        {
            case TouchPhase.Began:
                state.SetStart(pos, Aspect);
                OnEnter?.Invoke(state);
                break;
            case TouchPhase.Moved:
                state.SetCurrent(pos, Aspect);
                OnStay?.Invoke(state);
                break;
            case TouchPhase.Ended:
                state.SetExit(pos, Aspect);
                OnExit?.Invoke(state);
                break;
            default: break;
        }
    }


    /// <summary>
    /// 実機での入力
    /// </summary>
    private void InputForDevice()
    {
        foreach (var touch in Input.touches)
        {
            var id = touch.fingerId;
            if (!TouchStates.ContainsKey(id))
            {
                var newState = gameObject.AddComponent<TouchState>();
                newState.Limit = limit;
                TouchStates.Add(id, newState);
            }

            var state = TouchStates[id];
            var pos = touch.position;

            state.Phase.Value = touch.phase;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    state.SetStart(pos, Aspect);
                    OnEnter?.Invoke(state);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    state.SetCurrent(pos, Aspect);
                    OnStay?.Invoke(state);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    state.SetExit(pos, Aspect);
                    OnExit?.Invoke(state);
                    break;
                default: break;
            }
        }
    }
}
