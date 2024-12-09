using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    static EffectManager _main;
    public static EffectManager Main
    {
        get
        {
            if (_main) return _main;
            _main = FindObjectOfType<EffectManager>(true);
            _main.Init();
            return _main;
        }
    }


    [SerializeField] List<ParticleSystem> effects = new List<ParticleSystem>();


    readonly List<Tween> _effectPlayer = new List<Tween>();
    Canvas uiCanvas; // UIのCanvasを内部で生成して保持

    void Awake()
    {
        _ = Main;
    }

    void Init()
    {
        foreach (var effect in effects)
        {
            effect.gameObject.SetActive(false);


            var main = effect.main;
            var lifeTime = Mathf.Max(main.startLifetime.constantMax, main.startLifetimeMultiplier,
                main.startLifetime.constant, main.startLifetime.curveMultiplier);
            var delay = DOVirtual.DelayedCall(lifeTime,
                () => { effect.gameObject.SetActive(false); });
            delay.SetLink(effect.gameObject);
            delay.SetAutoKill(false);
            delay.Pause();

            _effectPlayer.Add(delay);
        }

        InitializeUICanvas();
    }

    public void Play(int effectNumber, Vector3 position, Vector3 lookDirection)
    {
        var effect = effects[effectNumber];
        effect.transform.position = position;
        effect.transform.rotation = Quaternion.LookRotation(lookDirection);
        Play(effectNumber);
    }

    public void Play(int effectNumber)
    {
        var effect = effects[effectNumber];
        DelayCancel(effectNumber);

        if (!effect.gameObject.activeSelf)
        {
            effect.gameObject.SetActive(true);
        }
        effect.Play();

        if (effect.main.loop) return;
        DelayDisable(effectNumber);
    }

    void InitializeUICanvas()
    {
        // CanvasのGameObjectを生成
        GameObject canvasObject = new GameObject("EffectUICanvas");
        uiCanvas = canvasObject.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // 画面上に重ねるUIとして設定

        // Canvas用の追加設定
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; // 解像度に合わせてスケーリング

        GraphicRaycaster raycaster = canvasObject.AddComponent<GraphicRaycaster>();
        canvasObject.layer = LayerMask.NameToLayer("UI"); // UIレイヤーに配置
    }

    // UIの上にエフェクトを再生するためのメソッド
    public void Play2D(int effectNumber, Vector3 screenPosition)
    {
        var effect = effects[effectNumber];

        // エフェクトの親をUI CanvasのTransformに設定し、スクリーン空間の位置をローカル空間に変換
        effect.transform.SetParent(uiCanvas.transform, false);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)uiCanvas.transform,
            screenPosition,
            uiCanvas.worldCamera,
            out Vector2 localPoint
        );

        effect.transform.localPosition = localPoint;
        effect.transform.localRotation = Quaternion.identity;
        effect.gameObject.SetActive(true);
        effect.Play();

        if (!effect.main.loop)
        {
            DelayDisable(effectNumber);
        }
    }

    void DelayCancel(int effect)
    {
        if (!_effectPlayer[effect].IsPlaying()) return;
        _effectPlayer[effect].Pause();
    }

    public void Stop(int effectNumber)
    {
        if (_effectPlayer[effectNumber].IsPlaying()) return;

        effects[effectNumber].Stop();
        DelayDisable(effectNumber);
    }

    void DelayDisable(int effectNumber)
    {
        _effectPlayer[effectNumber].Restart();
    }

    void OnDestroy()
    {
        _effectPlayer.Clear();
    }
}