using System;
using UnityEngine;
using System.Runtime.InteropServices;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class HapticManager : MonoSingleton<HapticManager>
{
    private static HapticManager _main;

    public static HapticManager Main
    {
        set => _main = value;
        get
        {
            if (_main) return _main;
            _main = FindObjectOfType<HapticManager>(true);
            return _main;
        }
    }
    
    [SerializeField] private LevelValue isMuteHaptic;

    private bool _isReservation;


    private HapticTypes _reservationType;
    private float _reservationDuration;

    private float _durationTimer = 0f;

    public bool IsActiveHaptic => !IsMuteHaptic;

    public bool IsMuteHaptic
    {
        get => isMuteHaptic.Value != 0;
        set => isMuteHaptic.Value = value ? 1 : 0;
    }

    public void ReservationHaptic(HapticTypes type, float duration)
    {
        if (IsMuteHaptic) return;

        _reservationType = type;
        _reservationDuration = duration;
        _isReservation = true;
    }


    public void Play(HapticTypes type)
    {
        if (IsMuteHaptic) return;
        MMVibrationManager.Haptic(type);
    }

    private void Play(HapticTypes type, float duration)
    {
        if (IsMuteHaptic) return;
        MMVibrationManager.Haptic(type);
        _durationTimer = duration;
    }


    private void Update()
    {
        if (IsMuteHaptic)
        {
            _isReservation = false;
            return;
        }

        if (_durationTimer > 0f)
        {
            _durationTimer -= Time.deltaTime;
            return;
        }

        if (_isReservation)
        {
            Play(_reservationType, _reservationDuration);
            _isReservation = false;
        }
    }
}