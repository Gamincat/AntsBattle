using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TouchState : MonoBehaviour
{
    [SerializeField] private float limit = 0.01f;

    private Vector2 _startScreenPos;
    private Vector2 _startViewPortPos;
    private Vector2 _currentScreenPos;
    private Vector2 _currentViewPortPos;
    private Vector2 _exitScreenPos;
    private Vector2 _exitViewPortPos;

    public Vector2 StartScreenPos => _startScreenPos;
    public Vector2 StartViewPortPos => _startViewPortPos;

    public Vector2 CurrentScreenPos => _currentScreenPos;

    public Vector2 CurrentViewPortPos => _currentViewPortPos;

    public Vector2 OffsetViewPort => _currentViewPortPos - _startViewPortPos;
    public Vector2 OffsetScreen => _currentScreenPos - _startScreenPos;

    public ReactiveProperty<TouchPhase> Phase { get; set; } = new ReactiveProperty<TouchPhase>(TouchPhase.Ended);

    public bool IsEnter => Phase.Value == TouchPhase.Began;
    public bool IsStay => Phase.Value == TouchPhase.Moved || Phase.Value == TouchPhase.Stationary;
    public bool IsExit => Phase.Value == TouchPhase.Ended || Phase.Value == TouchPhase.Canceled;

    public float Limit
    {
        get => limit;
        set => limit = value;
    }


    public void SetStart(Vector2 pos, float aspect)
    {
        _startScreenPos = pos;
        _startViewPortPos = pos;
        _startViewPortPos /= aspect;
    }


    public void SetCurrent(Vector2 pos, float aspect)
    {
        var (screenPos, viewPortPos) = LimitPos(pos, aspect);

        _currentScreenPos = screenPos;
        _currentViewPortPos = viewPortPos;
    }


    public void SetExit(Vector2 pos, float aspect)
    {
        var (screenPos, viewPortPos) = LimitPos(pos, aspect);

        _exitScreenPos = screenPos;
        _exitViewPortPos = viewPortPos;
    }


    private (Vector2 screenPos, Vector2 viewportPos) LimitPos(Vector2 pos, float aspect)
    {
        var viewPortPos = pos / aspect;
        var viewPortOffset = viewPortPos - _startViewPortPos;

        var limitOffset = viewPortOffset.normalized * Mathf.Min(viewPortOffset.magnitude, limit);


        return (_startScreenPos + (limitOffset * aspect), _startViewPortPos + limitOffset);
    }


    public void Copy(TouchState original)
    {
        _startScreenPos = original._startScreenPos;
        _startViewPortPos = original._startViewPortPos;


        _currentScreenPos = original._currentScreenPos;
        _currentViewPortPos = original._currentViewPortPos;
        _exitScreenPos = original._exitScreenPos;
        _exitViewPortPos = original._exitViewPortPos;

        Phase.Value = original.Phase.Value;
    }
}
