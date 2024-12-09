using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MultiTapInputter))]
public class DualInputter : MonoBehaviour
{
    [SerializeField] private MultiTapInputter multiInput;
    [SerializeField] private TouchState leftState;
    [SerializeField] private TouchState rightState;
    [SerializeField] private Transform leftObject;
    [SerializeField] private Transform rightObject;


    private TouchState CurrentLState { get; set; }
    private TouchState CurrentRState { get; set; }

    [SerializeField] private RectTransform leftStartImage;
    [SerializeField] private RectTransform leftCurrentImage;
    [SerializeField] private RectTransform rightStartImage;
    [SerializeField] private RectTransform rightCurrentImage;

    [SerializeField] private bool isFreezeStick = false;

    private float Aspect { get; set; }


    private void Awake()
    {
        var halfScreen = Screen.width / 2f;
        Aspect = Mathf.Max(Screen.width, Screen.height);

        multiInput.OnEnter += state =>
        {
            var leftDistance = Vector2.Distance(
                isFreezeStick ? leftStartImage.position : Camera.main.WorldToScreenPoint(leftObject.position),
                state.StartScreenPos);
            var rightDistance = Vector2.Distance(
                isFreezeStick ? rightStartImage.position : Camera.main.WorldToScreenPoint(rightObject.position),
                state.StartScreenPos);


            //タッチされたのが、左右どちらか判定
            if (leftDistance < rightDistance)
            {
                //スティック固定の場合は,指がスティックの移動範囲内に触れないと操作できない

                if (isFreezeStick)
                {
                    if (Vector2.Distance(state.StartViewPortPos, leftStartImage.position / multiInput.Aspect) >=
                        state.Limit) return;
                    state.SetStart(leftStartImage.position, Aspect);
                }

                if (CurrentLState == null) CurrentLState = state;
                leftState.Copy(state);
            }
            else
            {
                if (isFreezeStick)
                {
                    if (Vector2.Distance(state.StartViewPortPos, rightStartImage.position / multiInput.Aspect) >=
                        state.Limit) return;
                    state.SetStart(rightStartImage.position, Aspect);
                }

                if (CurrentRState == null) CurrentRState = state;
                rightState.Copy(state);
            }
        };

        multiInput.OnStay += state =>
        {
            //スティック固定の場合は、コントローラーを消さない
            if (!isFreezeStick)
            {
                leftStartImage.gameObject.SetActive(CurrentLState != null);
                leftCurrentImage.gameObject.SetActive(CurrentLState != null);
                rightStartImage.gameObject.SetActive(CurrentRState != null);
                rightCurrentImage.gameObject.SetActive(CurrentRState != null);
            }

            //情報を更新
            if (CurrentLState == state)
            {
                leftStartImage.position = state.StartScreenPos;
                leftCurrentImage.position = state.CurrentScreenPos;
                leftState.Copy(state);
            }

            if (CurrentRState == state)
            {
                rightStartImage.position = state.StartScreenPos;
                rightCurrentImage.position = state.CurrentScreenPos;
                rightState.Copy(state);
            }
        };

        multiInput.OnExit += state =>
        {
            if (CurrentLState == state)
            {
                leftState.Copy(CurrentLState);
                CurrentLState = null;

                if (!isFreezeStick)
                {
                    leftStartImage.gameObject.SetActive(false);
                    leftCurrentImage.gameObject.SetActive(false);
                }

                leftCurrentImage.position = leftStartImage.position;
            }

            if (CurrentRState == state)
            {
                rightState.Copy(CurrentRState);
                CurrentRState = null;
                if (!isFreezeStick)
                {
                    rightStartImage.gameObject.SetActive(false);
                    rightCurrentImage.gameObject.SetActive(false);
                }

                rightCurrentImage.position = rightStartImage.position;
            }
        };
    }
}
