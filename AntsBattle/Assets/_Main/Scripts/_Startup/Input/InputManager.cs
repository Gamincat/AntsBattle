using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputManager : MonoBehaviour
{
    static InputManager _main;
    public static InputManager Main
    {
        set => _main = value;
        get
        {
            if (_main) return _main;
            _main = FindObjectOfType<InputManager>(true);
            return _main;
        }
    }

    public delegate void TouchMethod(Vector2 startPoint, Vector2 inputVector, Vector2 offset, Vector2 currentPoint,
        Vector2 currentPixel, float offsetTime);

    public delegate void ClickMethod(Vector2 currentPoint, Vector2 currentPixel);

    [SerializeField] private Image panel;
    [SerializeField] private EventTrigger eventTrigger;

    public bool lockHorizontal;
    public bool lockVertical;
    public TouchMethod OnTouchEnter { get; set; }
    public TouchMethod OnTouchStay { get; set; }
    public TouchMethod OnTouchExit { get; set; }
    public ClickMethod OnClick { get; set; }
    
    bool IsActive
    {
        get => panel.enabled;
        set => panel.enabled = value;
    }

    float StartTime { get; set; }
    Vector2 StartPoint { get; set; }
    Vector2 PrevPoint { get; set; }
    int TargetID { get; set; }

    public bool EnterFlag { get; set; }

    Vector2 PosToViewPort(Vector2 pos)
    {
        var vp = Camera.main.ScreenToViewportPoint(pos);
        return vp;
    }

    public void Cancel()
    {
        EnterFlag = false;
    }

    public void TouchEnter(BaseEventData data)
    {
        StartTime = Time.time;
#if UNITY_EDITOR
        var currentPixel = Input.mousePosition;
#else
        if (Input.touches.Length != 1) return;
        var touch = Input.touches[0];
        TargetID = touch.fingerId;
        var currentPixel = touch.position;
#endif
        if (lockVertical) currentPixel.y = 0f;
        if (lockHorizontal) currentPixel.x = 0f;
        StartPoint = PosToViewPort(currentPixel);
        PrevPoint = StartPoint;

        OnTouchEnter?.Invoke(StartPoint, Vector2.zero, Vector2.zero, StartPoint, currentPixel, 0f);

        EnterFlag = true;
    }


    public void TouchStay(BaseEventData data)
    {
        if (!EnterFlag) return;

        var beginTime = Time.time;
#if UNITY_EDITOR
        var currentPixel = Input.mousePosition;

#else
        var touches = Array.FindAll(Input.touches, b => b.fingerId == TargetID);
        if (touches.Length <= 0) return;
        var touch = touches[0];
        var currentPixel = touch.position;
#endif
        if (lockVertical) currentPixel.y = 0f;
        if (lockHorizontal) currentPixel.x = 0f;
        var currentPoint = PosToViewPort(currentPixel);
        OnTouchStay?.Invoke(StartPoint, currentPoint - StartPoint, currentPoint - PrevPoint, currentPoint, currentPixel,
            beginTime - StartTime);
        PrevPoint = currentPoint;
    }

    public void TouchExit(BaseEventData data)
    {
        if (!EnterFlag) return;

        var beginTime = Time.time;
#if UNITY_EDITOR
        var currentPixel = Input.mousePosition;

#else
        var touches = Array.FindAll(Input.touches, b => b.fingerId == TargetID);
        if (touches.Length <= 0) return;
        var touch = touches[0];
        var currentPixel = touch.position;
#endif
        var currentPixelClick = currentPixel;
        var currentPointClick = PosToViewPort(currentPixelClick);

        if (lockVertical) currentPixel.y = 0f;
        if (lockHorizontal) currentPixel.x = 0f;
        var currentPoint = PosToViewPort(currentPixel);
        OnTouchExit?.Invoke(StartPoint, currentPoint - StartPoint, currentPoint - StartPoint, currentPoint,
            currentPixel, beginTime - StartTime);

        //Click
        if (beginTime - StartTime < 0.3f && Vector3.Distance(StartPoint, currentPoint) <= 0.05f)
        {
            OnClick?.Invoke(currentPointClick, currentPixelClick);
        }
    }

    public void SetActive(bool flag)
    {
        IsActive = flag;
    }


#if UNITY_EDITOR
    [ContextMenu("AutoInit")]
    private void AutoInit()
    {
        if (eventTrigger == null) eventTrigger = panel.AddComponent<EventTrigger>();
        eventTrigger.triggers.Clear();

        EditorUtility.SetDirty(gameObject);

        //eventTriggerのPointerDownにTouchEnter(),DragにTouchStay(),PointerUpにTouchExit()を設定。UnityEvent.AddListener()を用いる

        var enterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        enterEntry.callback.AddListener(TouchEnter);
        eventTrigger.triggers.Add(enterEntry);

        var stayEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag
        };
        stayEntry.callback.AddListener(TouchStay);
        eventTrigger.triggers.Add(stayEntry);

        var exitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        exitEntry.callback.AddListener(TouchExit);
        eventTrigger.triggers.Add(exitEntry);

        AssetDatabase.SaveAssets();
    }

#endif
}