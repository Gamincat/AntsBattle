using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GaminCat.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu]
public class AdjustParams : ScriptableObject
{
    private const string ScriptablePath = "AdjustParams";
    private static AdjustParams _instance;

    public static AdjustParams Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = Resources.Load<AdjustParams>(ScriptablePath);
            return _instance;
        }
    }

    [SerializeField] private string tokenIOS;
    [SerializeField] private string tokenAndroid;


    public string AppToken
    {
#if UNITY_IOS
        get => tokenIOS;
#else
        get => tokenAndroid;
#endif
    }
}