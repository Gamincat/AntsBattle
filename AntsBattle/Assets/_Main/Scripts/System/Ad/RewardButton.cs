using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private GameObject loadingCover;

    private TokenObject _token;

    public bool Interactable
    {
        get => button.interactable;
        set => button.interactable = value;
    }

    private void Awake()
    {
        loadingCover.SetActive(false);
    }

    public void Init(int rewardNum, Action startAction, Action endAction, Action okAction, Action ngAction,
        AdsManager.RewardType rewardType, int level)
    {
        _token = button.AddComponent<TokenObject>();

        void Click()
        {
            /*
            SoundManager.Main?.PlayClipFrom2DSpeaker(0);
            if (DataManager.Main.ticketValue.Value > 0)
            {
                TicketManager.Main.ShowDialog(okAction, () =>
                {
                    AdsManager.Instance.ShowRewardedAd(rewardNum, startAction, endAction, okAction, ngAction, button,
                        loadingCover, _token, rewardType,
                        level);
                });
            }
            else
            {
                AdsManager.Instance.ShowRewardedAd(rewardNum, startAction, endAction, okAction, ngAction, button,
                    loadingCover, _token, rewardType,
                    level);
            }*/
        }

        button.onClick.AddListener(Click);
    }

    public void SetActive(bool flag)
    {
        button.gameObject.SetActive(flag);
    }

    public bool GetActive()
    {
        return button.gameObject.activeSelf;
    }
}