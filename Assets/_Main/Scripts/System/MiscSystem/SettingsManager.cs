using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsManager : MonoSingleton<SettingsManager>
{
    [SerializeField] private GameObject settingWindow;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button restoreButton;
    [SerializeField] private GameObject restoreLoading;
    [SerializeField] private GameObject restoreComplete;

    [SerializeField] private Button privacySettingsButton;

    [SerializeField] private GameObject soundsObject;

    [SerializeField] private Button soundButton;
    [SerializeField] private Button hapticButton;
    [SerializeField] private Animator soundButtonAnimator;
    [SerializeField] private Animator hapticButtonAnimator;


    private static readonly int IsActive = Animator.StringToHash("IsActive");

    bool IsActiveAudio
    {
        get => !AudioManager.Singleton.IsMuteAudio;
        set
        {
            AudioManager.Singleton.IsMuteAudio = !value;
            AudioManager.Singleton.UpdateMute();
            soundButtonAnimator.SetBool(IsActive, IsActiveAudio);
        }
    }

    private bool IsActiveHaptic
    {
        set
        {
            HapticManager.Singleton.IsMuteHaptic = !value;
            hapticButtonAnimator.SetBool(IsActive, IsActiveHaptic);
        }
        get => !HapticManager.Singleton.IsMuteHaptic;
    }

    private void Awake()
    {
        soundButton.onClick.AddListener(() =>
        {
            if (AudioManager.Singleton)
            {
                IsActiveAudio = !IsActiveAudio;
                if (IsActiveAudio) AudioManager.Singleton.PlayClip(0);
            }
        });
        hapticButton.onClick.AddListener(() =>
        {
            IsActiveHaptic = !IsActiveHaptic;
            if (IsActiveHaptic) HapticManager.Singleton.Play(HapticTypes.MediumImpact);
        });

        settingWindow.SetActive(false);

        interactButton.onClick.AddListener(() =>
        {
            settingWindow.SetActive(!settingWindow.activeSelf);
            UpdateAnimators();
        });

        restoreButton.gameObject.SetActive(false);
#if UNITY_PURCHASING
        restoreButton.onClick.AddListener(() =>
        {
            restoreLoading.SetActive(true);
            IAPManager.Main.Restore((isComplete) =>
            {
                restoreLoading.SetActive(false);
                restoreComplete.SetActive(isComplete);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        });
#endif

#if GDPR
        privacySettingsButton.onClick.AddListener(GDPR.ShowDynamic);
        var privacySettingsFlag = GDPR.IsGDPRTarget;
#else
        var privacySettingsFlag = false;
#endif

        var restoreFlag = false;
#if UNITY_IOS
        restoreFlag = true;
#endif
#if UNITY_EDITOR
        restoreFlag = true;
        privacySettingsFlag = true;
#endif
#if !UNITY_PURCHASING
        restoreFlag = false;
#endif

        restoreButton.gameObject.SetActive(restoreFlag);
        privacySettingsButton.gameObject.SetActive(privacySettingsFlag);
        soundsObject.SetActive(true);
    }

    private void UpdateAnimators()
    {
        IsActiveAudio = IsActiveAudio;
        IsActiveHaptic = IsActiveHaptic;
        soundButtonAnimator.Play(IsActiveAudio ? "Switch_On" : "Switch_Off", 0, 0f);
        hapticButtonAnimator.Play(IsActiveHaptic ? "Switch_On" : "Switch_Off", 0, 0f);
    }
}