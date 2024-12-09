using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OfflinePause : MonoBehaviour
{
    [SerializeField] private GameObject errorDialog;
    [SerializeField] private Button closeButton;

    public float errorTimer;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseDialog);
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 機内モードなど、ネットワーク接続エラー状態
            errorTimer += Time.deltaTime;
            const float errorDuration = 2f;
            if (!(errorTimer > errorDuration)) return;
            InputManager.Main.SetActive(false);
            errorDialog.SetActive(true);
        }
        else
        {
            // ネットワーク接続OK状態
            errorTimer = 0f;
        }
    }


    private void CloseDialog()
    {
        errorDialog.SetActive(false);
        InputManager.Main.SetActive(true);
    }
}