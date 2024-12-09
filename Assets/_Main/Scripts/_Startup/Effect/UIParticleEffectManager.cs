using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIParticleEffectManager : MonoBehaviour
{
    [SerializeField] private Canvas uiPrefab;
    [SerializeField] private int pool = 30;
    [SerializeField] private float lifeTime = 0.3f;

    public class UIData
    {
        public UIData(Canvas canvas, TextMeshProUGUI text)
        {
            _canvas = canvas;
            _text = text;
        }


        public Canvas _canvas;
        public TextMeshProUGUI _text;
    }

    private List<UIData> uis = new List<UIData>();
    private int currentNum;
    private int currentComboValue;
    private int viewValue;


    private void Awake()
    {
        for (int i = 0; i < pool; i++)
        {
            var ui = Instantiate(uiPrefab);
            ui.transform.parent = transform;
            var text = ui.GetComponentInChildren<TextMeshProUGUI>();
            var uiData = new UIData(ui, text);
            uis.Add(uiData);
        }

        foreach (var ui in uis)
        {
            var canvas = ui._canvas;
            var text = ui._text;

            canvas.gameObject.SetActive(false);
            text.text = "";
        }
    }


    public void Play(Vector3 pos, int value, string headerText)
    {
        ForNext();
        currentComboValue = value;
        viewValue = value;

        var ui = uis[currentNum];
        var canvas = ui._canvas;
        var text = ui._text;
        canvas.gameObject.SetActive(true);
        canvas.transform.position = pos - (Vector3.down * 3f);
        canvas.transform.localScale = Vector3.one * 0.5f;

        canvas.transform.DOMove(pos, lifeTime).SetEase(Ease.OutBounce);
        canvas.transform.DOScale(Vector3.one, lifeTime).SetEase(Ease.OutBounce);
        text.text = headerText + value.ToString();
    }


    public void Combo(Vector3 pos, int value, string headerText)
    {
        var ui = uis[currentNum];
        var canvas = ui._canvas;
        if (!canvas.gameObject.activeSelf)
        {
            Play(pos, value, headerText);
            return;
        }

        currentComboValue += value;
        var text = ui._text;
        canvas.gameObject.SetActive(true);
        DOTween.To(() => viewValue, num =>
        {
            viewValue = num;
            text.text = headerText + viewValue.ToString();
        }, currentComboValue, lifeTime * 0.5f).SetEase(Ease.Linear);
    }


    public void HideAll()
    {
        foreach (var uiData in uis)
        {
            uiData._canvas.gameObject.SetActive(false);
        }
    }


    private void ForNext()
    {
        currentNum++;
        currentNum %= pool;
    }
}
