using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterstitialTimer : MonoBehaviour
{
    [SerializeField] private int viewTime = 5;
    [SerializeField] private TextMeshProUGUI timeViewer;
    [SerializeField] private Animator animator;
    [SerializeField] private bool rewardToReset = true;

    private bool _isShow;
    private int _timeSpan = 90;
    private float _timer = 0f;
    private static readonly int Active = Animator.StringToHash("IsActive");

    private int BeginTime { get; set; }

    private bool IsActive { get; set; } = false;


    private void Awake()
    {
        // シーンがロードされた時にイベントを呼び出すように設定します。
        SceneManager.sceneLoaded += OnSceneLoaded;
        AdsManager.Instance.OnAnyGetReward += ResetTimer;
        AdsManager.Instance.OnCloseInterstitial += ResetTimer;

        DontDestroyOnLoad(gameObject);
    }
    


// これがシーンが変わったときに呼び出されます。
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (!DataManager.Main) return;

        // //Level1ではインステを出さない、NoAd購入状態ではインステを出さない
        // if (DataManager.Main.currentLevel.Value == 0)// || IAPServer.GetActiveFlagLink(IAPDataSet.ProductName.NoADs))
        // {
        //     IsActive = false;
        //     return;
        // }

        IsActive = true;

        _timeSpan = 120;
    }

    private void Update()
    {
        if (!IsActive) return;

        _timer += Time.deltaTime;
        if (_timer >= _timeSpan)
        {
            ResetTimer();
            AdsManager.Instance.ShowInterstitialAd(0);
        }

        UpdateTimer();
    }


    private void OnDestroy()
    {
        AdsManager.Instance.OnAnyGetReward -= ResetTimer;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ResetTimer()
    {
        _timer = 0f;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        var value = _timeSpan - (int)_timer;

        if (BeginTime == value) return;
        BeginTime = value;
        timeViewer.text = BeginTime.ToString();

        SetView(value <= viewTime);
    }

    private void SetView(bool flag)
    {
        if (_isShow == flag) return;
        _isShow = flag;
        animator.SetBool(Active, flag);
    }
}