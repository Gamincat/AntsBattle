using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapBoard : MonoBehaviour
{
    public delegate void TapEvent(Vector2 pos, Vector2 vp);


    private readonly List<TapEvent> events = new List<TapEvent>();
    private RectTransform rectTransform;

    private Canvas canvas;


    private void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnTap()
    {
        var rect = rectTransform.rect;
        var canvasCamera = (canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;


        var touches = Input.touches;
        foreach (var touch in touches)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touch.position, canvasCamera,
                out var pos);

            if (!(rect.xMin < pos.x) || !(pos.x < rect.xMax) || !(rect.yMin < pos.y) || !(pos.y < rect.yMax)) continue;

            var px = pos.x - rect.xMin;
            var py = pos.y - rect.yMin;
            var sx = rect.xMax - rect.xMin;
            var sy = rect.yMax - rect.yMin;

            var viewPort = new Vector2(px / sx, py / sy);
            foreach (var tapEvent in events)
            {
                tapEvent(pos, viewPort);
            }
        }
    }

    public void AddEvent(TapEvent tapEvent)
    {
        events.Add(tapEvent);
    }
}